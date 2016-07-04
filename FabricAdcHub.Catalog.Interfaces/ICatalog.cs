using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using Microsoft.ServiceFabric.Services.Remoting;

namespace FabricAdcHub.Catalog.Interfaces
{
    public interface ICatalog : IService
    {
        Task UpdateSidInformation(string sid, Information information);

        Task<string[]> GetAllSids();

        Task<string[]> GetAllSids(IList<string> requiredFeatures, IList<string> prohibitedFeatures);

        Task<SidInformation[]> GetAllSidsInformation();
    }
}
