using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using Microsoft.ServiceFabric.Services.Remoting;

namespace FabricAdcHub.Catalog.Interfaces
{
    public interface ICatalog : IService
    {
        Task Enable();

        Task Disable();

        Task<ReservedSid> ReserveSid();

        Task ReleaseSid(string sid);

        Task BroadcastNewSidInformation(string sid);

        Task BroadcastCommand(string fromSid, Command command);

        Task FeatureBroadcastCommand(string fromSid, Command command);

        Task UpdateSidInformation(string sid, HashSet<string> features);
    }
}
