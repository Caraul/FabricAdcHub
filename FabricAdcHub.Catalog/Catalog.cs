using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SidDictionary = Microsoft.ServiceFabric.Data.Collections.IReliableDictionary<string, FabricAdcHub.Catalog.FullSidInformation>;

namespace FabricAdcHub.Catalog
{
    internal sealed class Catalog : StatefulService, ICatalog
    {
        public Catalog(StatefulServiceContext context)
            : base(context)
        {
        }

        public async Task UpdateSidInformation(string sid, Information information)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                await (await Sids).SetAsync(tx, sid, new FullSidInformation { Features = information.Features.Value.ToList(), Information = information.ToText() });
                await tx.CommitAsync();
            }
        }

        public async Task<string[]> GetAllSids()
        {
            var maybeSids = await MaybeSids;
            if (!maybeSids.HasValue)
            {
                return new string[0];
            }

            var allSids = new List<string>();
            using (var tx = StateManager.CreateTransaction())
            {
                var enumerable = await maybeSids.Value.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        allSids.Add(enumerator.Current.Key);
                    }
                }
            }

            return allSids.ToArray();
        }

        public async Task<SidInformation[]> GetAllSidsInformation()
        {
            var maybeSids = await MaybeSids;
            if (!maybeSids.HasValue)
            {
                return new SidInformation[0];
            }

            var allSids = new List<SidInformation>();
            using (var tx = StateManager.CreateTransaction())
            {
                var enumerable = await maybeSids.Value.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        allSids.Add(new SidInformation
                        {
                            Sid = enumerator.Current.Key,
                            Information = enumerator.Current.Value.Information
                        });
                    }
                }
            }

            return allSids.ToArray();
        }

        public async Task<string[]> GetAllSids(IList<string> requiredFeatures, IList<string> prohibitedFeatures)
        {
            var maybeSids = await MaybeSids;
            if (!maybeSids.HasValue)
            {
                return new string[0];
            }

            var allSids = new List<string>();
            using (var tx = StateManager.CreateTransaction())
            {
                var enumerable = await maybeSids.Value.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        var features = enumerator.Current.Value.Features;
                        if (!requiredFeatures.Except(features).Any() && !features.Intersect(prohibitedFeatures).Any())
                        {
                            allSids.Add(enumerator.Current.Key);
                        }
                    }
                }
            }

            return allSids.ToArray();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener, listenOnSecondary: true) };
        }

        private Task<SidDictionary> Sids => StateManager.GetOrAddAsync<SidDictionary>("sids");

        private Task<ConditionalValue<SidDictionary>> MaybeSids => StateManager.TryGetAsync<SidDictionary>("sids");
    }
}
