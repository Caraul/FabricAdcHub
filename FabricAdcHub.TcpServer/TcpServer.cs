using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FabricAdcHub.TcpServer
{
    internal sealed class TcpServer : StatelessService
    {
        public TcpServer(StatelessServiceContext context)
            : base(context)
        {
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context => new TcpCommunicationListener(context)) };
        }
    }
}
