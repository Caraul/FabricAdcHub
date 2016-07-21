using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace FabricAdcHub.Catalog.Interfaces
{
    public interface ICatalog : IService
    {
        Task<ReservedSid> ReserveSid();

        Task ReleaseSid(string sid);

        Task BroadcastNewSidInformation(string newSid, string newSidInformation);

        Task BroadcastMessage(string fromSid, string message);

        Task FeatureBroadcastMessage(string fromSid, List<string> requiredFeatures, List<string> excludedFeatures, string message);

        Task UpdateSidInformation(string sid, HashSet<string> features);
    }
}
