using System;
using System.Diagnostics.CodeAnalysis;
using System.Fabric;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

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
                            try
                            {
                                await CreateAdcClient(tcpClient);
                            }
                            catch (Exception exception)
                            {
                                ServiceEventSource.Current.ServiceRequestFailed(tcpClient.Client.RemoteEndPoint.ToString(), exception.ToString());
                                tcpClient.Close();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ServiceEventSource.Current.TcpExchangeFailed(exception.ToString());
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
