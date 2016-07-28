using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Sender.Interfaces;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.TcpServer
{
    internal class AdcClient : ISenderEvents, IDisposable
    {
        public AdcClient(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public async Task Open(IPAddress clientIPv4, IPAddress clientIPv6)
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"));
            var reservation = await catalog.ReserveSid();
            if (reservation.Error != Status.ErrorCode.NoError)
            {
                var command = new Status(new InformationMessageHeader(), Status.ErrorSeverity.Fatal, reservation.Error, "All your connection are reject by us.");
                await SendCommand(command);
                Dispose();
                return;
            }

            _sid = reservation.Sid;
            var user = GetUser(_sid);
            await user.Open(clientIPv4, clientIPv6);
            await GetSender(_sid).SubscribeAsync<ISenderEvents>(this);
            _adcReader = RunInfiniteRead();
        }

        public void MessageAvailable(string message)
        {
            try
            {
                SendAvailableMessage(message);
            }
            catch (IOException exception)
            {
                ServiceEventSource.Current.TcpExchangeFailed(exception.ToString());
                DisconnectOnWrite().Wait();
            }
        }

        public void Closed()
        {
            DisconnectOnClosed().Wait();
        }

        public void Dispose()
        {
            _adcListenerCancellation.Dispose();
            _tcpClient.Dispose();
        }

        private async Task RunInfiniteRead()
        {
            try
            {
                if (!await InfiniteRead())
                {
                    await DisconnectOnRead();
                }
            }
            catch (Exception exception)
            {
                ServiceEventSource.Current.TcpExchangeFailed(exception.ToString());
                await DisconnectOnRead();
            }
        }

        private async Task<bool> InfiniteRead()
        {
            var textBuffer = new StringBuilder();
            var dataBuffer = new byte[256];
            while (!_adcListenerCancellation.IsCancellationRequested)
            {
                while (true)
                {
                    var readCount = await _tcpClient.GetStream().ReadAsync(dataBuffer, 0, dataBuffer.Length, _adcListenerCancellation.Token);
                    if (readCount == 0)
                    {
                        return false;
                    }

                    var text = Encoding.UTF8.GetString(dataBuffer, 0, readCount);
                    textBuffer.Append(text);
                    if (text.IndexOf('\n') != -1)
                    {
                        break;
                    }
                }

                var bufferedText = textBuffer.ToString();
                var messages = bufferedText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (bufferedText.Last() != '\n')
                {
                    var totalMessageLength = 0;
                    for (var index = 0; index != messages.Length - 1; index++)
                    {
                        await ReceiveMessage(messages[index]);
                        totalMessageLength += messages[index].Length + 1;
                    }

                    textBuffer.Remove(0, totalMessageLength);
                }
                else
                {
                    for (var index = 0; index != messages.Length; index++)
                    {
                        await ReceiveMessage(messages[index]);
                    }

                    textBuffer.Clear();
                }
            }

            return true;
        }

        private async Task SendCommand(Command command)
        {
            var message = command.ToMessage();
            ServiceEventSource.Current.AdcMessageSent(message);

            var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
            await _tcpClient.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length);
        }

        private void SendAvailableMessage(string message)
        {
            ServiceEventSource.Current.AdcMessageSent(message);

            var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
            _tcpClient.GetStream().Write(messageBytes, 0, messageBytes.Length);
        }

        private async Task ReceiveMessage(string message)
        {
            ServiceEventSource.Current.AdcMessageReceived(message);

            await GetUser(_sid).ProcessMessage(message);
        }

        private async Task DisconnectOnRead()
        {
            ServiceEventSource.Current.TcpExchangeEnded("Read failed - " + _tcpClient.Client.RemoteEndPoint);

            await GetSender(_sid).UnsubscribeAsync<ISenderEvents>(this);
            Dispose();
            await GetUser(_sid).CloseOnDisconnect();
        }

        private async Task DisconnectOnWrite()
        {
            ServiceEventSource.Current.TcpExchangeEnded("Write failed - " + _tcpClient.Client.RemoteEndPoint);

            // unsubscription is unavailable here inside event handling
            _adcListenerCancellation.Cancel();
            await _adcReader;
            Dispose();
            await GetUser(_sid).CloseOnDisconnect();
        }

        private async Task DisconnectOnClosed()
        {
            ServiceEventSource.Current.TcpExchangeEnded("Closed - " + _tcpClient.Client.RemoteEndPoint);

            await GetSender(_sid).UnsubscribeAsync<ISenderEvents>(this);
            _adcListenerCancellation.Cancel();
            await _adcReader;
            Dispose();
        }

        private static IUser GetUser(string sid)
        {
            return ActorProxy.Create<IUser>(new ActorId(sid));
        }

        private static ISender GetSender(string sid)
        {
            return ActorProxy.Create<ISender>(new ActorId(sid));
        }

        private readonly TcpClient _tcpClient;
        private readonly CancellationTokenSource _adcListenerCancellation = new CancellationTokenSource();
        private Task _adcReader;
        private string _sid;
    }
}
