using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FabricAdcHub.TcpServer
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                ServiceRuntime.RegisterServiceAsync(
                    "TcpServerType",
                    context => new TcpServer(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(TcpServer).Name);

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
