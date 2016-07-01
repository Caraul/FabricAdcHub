using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FabricAdcHub.TcpClients
{
    internal sealed class TcpClients : StatelessService
    {
        public TcpClients(StatelessServiceContext context)
            : base(context)
        {
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }
    }
}
