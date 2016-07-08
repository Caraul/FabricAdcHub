﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using DeletedSidsQueue = Microsoft.ServiceFabric.Data.Collections.IReliableQueue<string>;
using SidDictionary = Microsoft.ServiceFabric.Data.Collections.IReliableDictionary<string, FabricAdcHub.Catalog.SidInformation>;
using StateDictionary = Microsoft.ServiceFabric.Data.Collections.IReliableDictionary<string, bool>;

namespace FabricAdcHub.Catalog
{
    internal sealed class Catalog : StatefulService, ICatalog
    {
        public Catalog(StatefulServiceContext context)
            : base(context)
        {
        }

        public async Task Enable()
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var state = await State;
                await state.SetAsync(tx, IsEnabled, true);
                await tx.CommitAsync();
            }
        }

        public async Task Disable()
        {
            var state = await State;
            using (var tx = StateManager.CreateTransaction())
            {
                await state.SetAsync(tx, IsEnabled, false);
                await tx.CommitAsync();
            }

            await DisconnectAll();
            var sids = await Sids;
            await sids.ClearAsync();
        }

        public async Task<ReservedSid> ReserveSid()
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var state = await State;
                var isEnabled = await state.TryGetValueAsync(tx, IsEnabled);
                if (!isEnabled.HasValue || !isEnabled.Value)
                {
                    return new ReservedSid(Status.ErrorCode.HubDisabled);
                }

                var sids = await Sids;
                var newSid = CreateNewSid();
                while (!await sids.TryAddAsync(tx, newSid, null))
                {
                    if (await sids.GetCountAsync(tx) == MaxSessionsCount)
                    {
                        return new ReservedSid(Status.ErrorCode.HubFull);
                    }

                    newSid = CreateNewSid();
                }

                await tx.CommitAsync();
                return new ReservedSid(newSid);
            }
        }

        public async Task ReleaseSid(string sid)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                await DisconnectUser(sid);
                var deletedSids = await DeletedSids;
                await deletedSids.EnqueueAsync(tx, sid);
            }
        }

        public async Task BroadcastSidInformation(string newSid)
        {
            var newUser = ActorProxy.Create<IUser>(new ActorId(newSid));
            var newInformation = await newUser.GetInformation();
            await ForAllSids(
                await Sids,
                async sid =>
                {
                    if (sid == newSid)
                    {
                        return;
                    }

                    var user = ActorProxy.Create<IUser>(new ActorId(sid));
                    await user.SendCommand(newInformation);
                    var information = await user.GetInformation();
                    await newUser.SendCommand(information);
                });
            await newUser.SendCommand(newInformation);
        }

        public async Task BroadcastCommand(string fromSid, Command command)
        {
            var maybeSids = await MaybeSids;
            if (!maybeSids.HasValue)
            {
                return;
            }

            await ForAllSids(
                maybeSids.Value,
                async sid =>
                {
                    if (sid != fromSid)
                    {
                        var user = ActorProxy.Create<IUser>(new ActorId(sid));
                        await user.SendCommand(command);
                    }
                });
        }

        public async Task FeatureBroadcastCommand(string fromSid, Command command)
        {
            var maybeSids = await MaybeSids;
            if (!maybeSids.HasValue)
            {
                return;
            }

            var featureMessageType = (FeatureBroadcastMessageHeader)command.Header;
            await ForAllSids(
                maybeSids.Value,
                async (sid, data) =>
                {
                    if (sid != fromSid
                        && !featureMessageType.RequiredFeatures.Except(data.Features).Any()
                        && !data.Features.Intersect(featureMessageType.ExcludedFeatures).Any())
                    {
                        var user = ActorProxy.Create<IUser>(new ActorId(sid));
                        await user.SendCommand(command);
                    }
                });
        }

        public async Task UpdateSidInformation(string sid, HashSet<string> features)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                await (await Sids).SetAsync(tx, sid, new SidInformation { Features = features });
                await tx.CommitAsync();
            }
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
                    var maybeSid = await deletedSids.TryDequeueAsync(tx, TimeSpan.MaxValue, cancellationToken);
                    if (maybeSid.HasValue)
                    {
                        var sid = maybeSid.Value;
                        await DeleteUser(sid);
                        var sids = await Sids;
                        await sids.TryRemoveAsync(tx, sid);
                        await tx.CommitAsync();
                    }
                }
            }
        }

        private Task<StateDictionary> State => StateManager.GetOrAddAsync<StateDictionary>("state");

        private Task<SidDictionary> Sids => StateManager.GetOrAddAsync<SidDictionary>("sids");

        private Task<ConditionalValue<SidDictionary>> MaybeSids => StateManager.TryGetAsync<SidDictionary>("sids");

        private Task<DeletedSidsQueue> DeletedSids => StateManager.GetOrAddAsync<DeletedSidsQueue>("deletedsids");

        private async Task InitializeState()
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var state = await State;
                await state.AddAsync(tx, IsEnabled, true);
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
            var quitCommand = new Quit(new InformationMessageHeader(), disconnectingSid);
            await ForAllSids(
                await Sids,
                async sid =>
                {
                    var user = ActorProxy.Create<IUser>(new ActorId(sid));
                    await user.SendCommand(quitCommand);
                });
        }

        private async Task DisconnectAll()
        {
            await ForAllSids(
                await Sids,
                async sid =>
                {
                    var user = ActorProxy.Create<IUser>(new ActorId(sid));
                    await user.Disconnect(DisconnectReason.HubIsDisabled);
                    var quitCommand = new Quit(new InformationMessageHeader(), sid);
                    await user.SendCommand(quitCommand);
                    await DeleteUser(sid);
                });
        }

        private async Task ForAllSids(SidDictionary sids, Func<string, Task> process)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var enumerable = await sids.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        await process(enumerator.Current.Key);
                    }
                }
            }
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
                        await process(enumerator.Current.Key, enumerator.Current.Value);
                    }
                }
            }
        }

        private static async Task DeleteUser(string sid)
        {
            var actorId = new ActorId(sid);
            var actorServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/FabricAdcHub/UserActorService"), actorId);
            await actorServiceProxy.DeleteActorAsync(actorId, CancellationToken.None);
        }

        private const string IsEnabled = "enabled";
        private const int MaxSessionsCount = 32 * 32 * 32 * 32;
        private readonly Random _random = new Random();
    }
}
