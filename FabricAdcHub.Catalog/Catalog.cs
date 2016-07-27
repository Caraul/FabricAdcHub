using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;
using FabricAdcHub.Sender.Interfaces;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using DeletedSidsQueue = Microsoft.ServiceFabric.Data.Collections.IReliableQueue<string>;
using SidDictionary = Microsoft.ServiceFabric.Data.Collections.IReliableDictionary<string, FabricAdcHub.Catalog.SidInformation>;

namespace FabricAdcHub.Catalog
{
    internal sealed class Catalog : StatefulService, ICatalog
    {
        public Catalog(StatefulServiceContext context)
            : base(context)
        {
        }

        public async Task<ReservedSid> ReserveSid()
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var sids = await Sids;
                var newSid = CreateNewSid();
                while (!await sids.TryAddAsync(tx, newSid, null))
                {
                    if (await sids.GetCountAsync(tx) == MaxSessionsCount)
                    {
                        ServiceEventSource.Current.SidReservationFailed(Status.ErrorCode.HubFull.ToString());
                        return ReservedSid.CreateAsError(Status.ErrorCode.HubFull);
                    }

                    newSid = CreateNewSid();
                }

                await tx.CommitAsync();

                ServiceEventSource.Current.SidReserved(newSid);
                return ReservedSid.CreateAsSid(newSid);
            }
        }

        public async Task ReleaseSid(string sid)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                await DisconnectUser(sid);
                var deletedSids = await DeletedSids;
                await deletedSids.EnqueueAsync(tx, sid);
                await tx.CommitAsync();
            }
        }

        public async Task ExposeSid(string exposingSid, string exposingSidInformation)
        {
            var exposingSender = ActorProxy.Create<ISender>(new ActorId(exposingSid));
            using (var tx = StateManager.CreateTransaction())
            {
                var sids = await Sids;
                await ForAllSids(
                    sids,
                    async sid =>
                    {
                        if (sid != exposingSid)
                        {
                            var sender = ActorProxy.Create<ISender>(new ActorId(sid));
                            await sender.SendMessage(exposingSidInformation);
                            var information = await GetUser(sid).GetInformationMessage();
                            await exposingSender.SendMessage(information);
                        }
                    });

                await exposingSender.SendMessage(exposingSidInformation);

                var maybeData = await sids.TryGetValueAsync(tx, exposingSid);
                await sids.SetAsync(tx, exposingSid, new SidInformation { Nick = maybeData.Value.Nick, IsExposed = true, Features = maybeData.Value.Features });

                await tx.CommitAsync();
            }
        }

        public async Task BroadcastMessage(string fromSid, string message)
        {
            ServiceEventSource.Current.Broadcasted(message);

            var maybeSids = await MaybeSids;
            if (!maybeSids.HasValue)
            {
                return;
            }

            await ForAllSids(
                maybeSids.Value,
                async sid =>
                {
                    await GetSender(sid).SendMessage(message);
                });
        }

        public async Task FeatureBroadcastMessage(string fromSid, List<string> requiredFeatures, List<string> excludedFeatures, string message)
        {
            ServiceEventSource.Current.FeatureBroadcasted(message);

            var maybeSids = await MaybeSids;
            if (!maybeSids.HasValue)
            {
                return;
            }

            await ForAllSids(
                maybeSids.Value,
                async (sid, information) =>
                {
                    if (!requiredFeatures.Except(information.Features).Any()
                        && !information.Features.Intersect(excludedFeatures).Any())
                    {
                        await GetSender(sid).SendMessage(message);
                    }
                });
        }

        public async Task<bool> ReserveNick(string sid, string nick)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var sids = await Sids;
                var enumerable = await sids.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        if (enumerator.Current.Value != null && enumerator.Current.Value.Nick == nick)
                        {
                            return false;
                        }
                    }
                }

                var maybeData = await sids.TryGetValueAsync(tx, sid);
                if (maybeData.HasValue && maybeData.Value != null)
                {
                    await sids.SetAsync(tx, sid, new SidInformation { Nick = nick, Features = maybeData.Value.Features });
                }
                else
                {
                    await sids.SetAsync(tx, sid, new SidInformation { Nick = nick });
                }

                await tx.CommitAsync();
                return true;
            }
        }

        public async Task UpdateSidInformation(string sid, HashSet<string> features)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var sids = await Sids;
                var maybeData = await sids.TryGetValueAsync(tx, sid);
                await sids.SetAsync(tx, sid, new SidInformation { Nick = maybeData.Value.Nick, IsExposed = maybeData.Value.IsExposed, Features = features });
                await tx.CommitAsync();
            }

            ServiceEventSource.Current.InformationUpdated(sid);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener, listenOnSecondary: true) };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await InitializeState();
            var deletedSids = await DeletedSids;
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var tx = StateManager.CreateTransaction())
                {
                    try
                    {
                        var maybeSid = await deletedSids.TryDequeueAsync(tx);
                        if (maybeSid.HasValue)
                        {
                            var sid = maybeSid.Value;
                            await DeleteUser(sid);
                            var sids = await Sids;
                            await sids.TryRemoveAsync(tx, sid);
                            await tx.CommitAsync();
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                        }
                    }
                    catch (TimeoutException)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(60, 180)));
                    }
                }
            }
        }

        private Task<SidDictionary> Sids => StateManager.GetOrAddAsync<SidDictionary>(StoredSids);

        private Task<ConditionalValue<SidDictionary>> MaybeSids => StateManager.TryGetAsync<SidDictionary>(StoredSids);

        private Task<DeletedSidsQueue> DeletedSids => StateManager.GetOrAddAsync<DeletedSidsQueue>(StoredDeletedSids);

        private async Task InitializeState()
        {
            using (var tx = StateManager.CreateTransaction())
            {
                await Sids;
                await DeletedSids;
                await tx.CommitAsync();
            }
        }

        private string CreateNewSid()
        {
            var bytes = new byte[3];
            _random.NextBytes(bytes);
            return AdcBase32Encoder.Encode(bytes).Substring(0, 4);
        }

        private async Task DisconnectUser(string disconnectingSid)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var sids = await Sids;
                var maybeData = await sids.TryGetValueAsync(tx, disconnectingSid);
                if (maybeData.HasValue && maybeData.Value.IsExposed)
                {
                    var quitCommand = new Quit(new InformationMessageHeader(), disconnectingSid);
                    await ForAllSids(
                        await Sids,
                        async sid =>
                        {
                            if (sid == disconnectingSid)
                            {
                                return;
                            }

                            await GetSender(sid).SendMessage(quitCommand.ToMessage());
                        });
                }
            }
        }

        private Task ForAllSids(SidDictionary sids, Func<string, Task> process)
        {
            return ForAllSids(sids, (sid, _) => process(sid));
        }

        private async Task ForAllSids(SidDictionary sids, Func<string, SidInformation, Task> process)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var enumerable = await sids.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        if (enumerator.Current.Value != null && enumerator.Current.Value.IsExposed)
                        {
                            await process(enumerator.Current.Key, enumerator.Current.Value);
                        }
                    }
                }
            }
        }

        private static IUser GetUser(string sid)
        {
            return ActorProxy.Create<IUser>(new ActorId(sid));
        }

        private static ISender GetSender(string sid)
        {
            return ActorProxy.Create<ISender>(new ActorId(sid));
        }

        private static async Task DeleteUser(string sid)
        {
            ServiceEventSource.Current.SidReleased(sid);

            var actorId = new ActorId(sid);
            var userServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/FabricAdcHub.ServiceFabric/UserActorService"), actorId);
            await userServiceProxy.DeleteActorAsync(actorId, CancellationToken.None);
            var senderServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/FabricAdcHub.ServiceFabric/SenderActorService"), actorId);
            await senderServiceProxy.DeleteActorAsync(actorId, CancellationToken.None);
        }

        private const string StoredSids = "sids";
        private const string StoredDeletedSids = "deletedsids";
        private const int MaxSessionsCount = 32 * 32 * 32 * 32;
        private readonly Random _random = new Random();
    }
}
