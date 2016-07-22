using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace FabricAdcHub.Catalog.Interfaces
{
    public interface ICatalog : IService
    {
        Task<ReservedSid> ReserveSid();

        Task<bool> ReserveNick(string sid, string nick);

        Task<List<string>> ExposeSid(string exposedSid);

        Task ReleaseSid(string sid);

        Task BroadcastMessage(string fromSid, string message);

        Task FeatureBroadcastMessage(string fromSid, List<string> requiredFeatures, List<string> excludedFeatures, string message);

        Task UpdateSidInformation(string sid, HashSet<string> features);
    }
}
