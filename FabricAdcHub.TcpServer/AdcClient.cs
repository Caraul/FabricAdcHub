using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.TcpServer
{
    internal class AdcClient : IUserEvents, IDisposable
    {
        public AdcClient(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public async Task Open(IPAddress clientIPv4, IPAddress clientIPv6)
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"));
            var reservation = await catalog.ReserveSid();
            if (reservation.Error != Status.ErrorCode.NoError)
            {
                var command = new Status(new InformationMessageHeader(), Status.ErrorSeverity.Fatal, reservation.Error, "All your connection are reject by us.");
                await SendCommand(command);
                Dispose();
                return;
            }

            _sid = reservation.Sid;
            var user = ActorProxy.Create<IUser>(new ActorId(_sid));
            await user.Open(clientIPv4, clientIPv6);
            RunInfiniteRead();
            await user.SubscribeAsync<IUserEvents>(this);
        }

        public void MessageAvailable(string message)
        {
            try
            {
                SendMessage(message);
            }
            catch (Exception exception)
            {
                ServiceEventSource.Current.TcpExchangeFailed(exception.ToString());
                var user = ActorProxy.Create<IUser>(new ActorId(_sid));
                user.Disconnect(DisconnectReason.NetworkError).Wait();
            }
        }

        public void Closed()
        {
            ServiceEventSource.Current.TcpExchangeEnded(_tcpClient.Client.RemoteEndPoint.ToString());

            var user = ActorProxy.Create<IUser>(new ActorId(_sid));
            user.UnsubscribeAsync<IUserEvents>(this).Wait();
            Dispose();
        }

        public void Dispose()
        {
            _adcListenerCancellation.Cancel();
            _adcListener?.Wait();
            _adcListener?.Dispose();
            _adcListenerCancellation.Dispose();
            _tcpClient.Dispose();
        }

        private void RunInfiniteRead()
        {
            _adcListener = Task.Run(
                async () =>
                {
                    try
                    {
                        await InfiniteRead();
                    }
                    catch (Exception)
                    {
                        var user = ActorProxy.Create<IUser>(new ActorId(_sid));
                        await user.Disconnect(DisconnectReason.NetworkError);
                    }
                },
                _adcListenerCancellation.Token);
        }

        private async Task InfiniteRead()
        {
            var textBuffer = new StringBuilder();
            var dataBuffer = new byte[256];
            while (!_adcListenerCancellation.IsCancellationRequested)
            {
                while (true)
                {
                    var readCount = await _tcpClient.GetStream().ReadAsync(dataBuffer, 0, dataBuffer.Length, _adcListenerCancellation.Token);
                    var text = Encoding.UTF8.GetString(dataBuffer, 0, readCount);
                    textBuffer.Append(text);
                    if (text.IndexOf('\n') != -1)
                    {
                        break;
                    }
                }

                var user = ActorProxy.Create<IUser>(new ActorId(_sid));
                var bufferedText = textBuffer.ToString();
                var messages = bufferedText.Split('\n');
                if (bufferedText.Last() != '\n')
                {
                    var totalMessageLength = 0;
                    for (var index = 0; index != messages.Length - 1; index++)
                    {
                        await ReceiveMessage(user, messages[index]);
                        totalMessageLength += messages[index].Length + 1;
                    }

                    textBuffer.Remove(0, totalMessageLength);
                }
                else
                {
                    for (var index = 0; index != messages.Length; index++)
                    {
                        await ReceiveMessage(user, messages[index]);
                    }

                    textBuffer.Clear();
                }
            }
        }

        private async Task SendCommand(Command command)
        {
            var message = command.ToText();
            ServiceEventSource.Current.AdcMessageSent(message);

            var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
            await _tcpClient.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length);
        }

        private void SendMessage(string message)
        {
            ServiceEventSource.Current.AdcMessageSent(message);

            var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
            _tcpClient.GetStream().Write(messageBytes, 0, messageBytes.Length);
        }

        private static async Task ReceiveMessage(IUser user, string message)
        {
            ServiceEventSource.Current.AdcMessageReceived(message);

            await user.ProcessMessage(message);
        }

        private readonly TcpClient _tcpClient;
        private readonly CancellationTokenSource _adcListenerCancellation = new CancellationTokenSource();
        private Task _adcListener;
        private string _sid;
    }
}
