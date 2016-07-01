using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FabricAdcHub.TcpClients
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                ServiceRuntime.RegisterServiceAsync(
                    "TcpClientsType",
                    context => new TcpClients(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(TcpClients).Name);

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
