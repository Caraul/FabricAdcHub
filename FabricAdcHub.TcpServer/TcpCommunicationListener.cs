using System;
using System.Diagnostics.CodeAnalysis;
using System.Fabric;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.TcpServer
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Not needed")]
    public sealed class TcpCommunicationListener : ICommunicationListener
    {
        public TcpCommunicationListener(StatelessServiceContext context)
        {
            _context = context;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var endpoint = _context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");

            _tcpListener = Task.Run(
                async () =>
                {
                    var tcpListener = TcpListener.Create(endpoint.Port);
                    try
                    {
                        tcpListener.Start();
                        while (!_tcpListenerCancellation.IsCancellationRequested)
                        {
                            var tcpClient = await tcpListener.AcceptTcpClientAsync();
                            await CreateAdcClient(tcpClient);
                        }
                    }
                    finally
                    {
                        tcpListener.Stop();
                    }
                },
                _tcpListenerCancellation.Token);

            var uriPrefix = $"{endpoint.Protocol}://+:{endpoint.Port}";
            var uriPublished = uriPrefix.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            return Task.FromResult(uriPublished);
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            _tcpListenerCancellation.Cancel();
            await _tcpListener;

            _tcpListenerCancellation.Dispose();
            _tcpListener?.Dispose();
        }

        public void Abort()
        {
            _tcpListenerCancellation.Cancel();
        }

        public async Task CreateAdcClient(TcpClient tcpClient)
        {
            ServiceEventSource.Current.TcpExchangeStarted(tcpClient.Client.RemoteEndPoint.ToString());

            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"));
            var reservation = await catalog.ReserveSid();
            if (reservation.Error != Status.ErrorCode.NoError)
            {
                var command = new Status(new InformationMessageHeader(), Status.ErrorSeverity.Fatal, reservation.Error, "All your connection are reject by us.");
                var message = command.ToText();
                var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
                ServiceEventSource.Current.AdcMessageSent(message);
                await tcpClient.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length);
                tcpClient.Close();
                ServiceEventSource.Current.TcpExchangeEnded(tcpClient.Client.RemoteEndPoint.ToString());
                return;
            }

            var adcClient = new AdcClient(tcpClient);
            await adcClient.Open(
                ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.MapToIPv4(),
                ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.MapToIPv6());
        }

        private readonly StatelessServiceContext _context;
        private readonly CancellationTokenSource _tcpListenerCancellation = new CancellationTokenSource();
        private Task _tcpListener;
    }
}
