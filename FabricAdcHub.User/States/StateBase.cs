using System;
using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.User.States
{
    internal abstract class StateBase
    {
        public abstract State State { get; }

        public abstract Task<State> ProcessCommand(Command command);

        protected StateBase(User user)
        {
            User = user;
        }

        protected User User { get; }

        protected async Task BroadcastCommand(Command message)
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            var allSids = await catalog.GetAllSids();
            for (var index = 0; index != allSids.Length; index++)
            {
                var user = ActorProxy.Create<IUser>(new ActorId(allSids[index]));
                await user.SendMessage(message);
            }
        }

        protected async Task DirectCommand(Command message)
        {
            var directMessageType = (DirectMessageHeader)message.Header;
            var user = ActorProxy.Create<IUser>(new ActorId(directMessageType.TargetSid));
            await user.SendMessage(message);
        }

        protected async Task FeatureBroadcastCommand(Command message)
        {
            var featureMessageType = (FeatureBroadcastMessageHeader)message.Header;
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            var allSids = await (
                featureMessageType.RequiredFeatures.Any() || featureMessageType.ExcludedFeatures.Any()
                ? catalog.GetAllSids(featureMessageType.RequiredFeatures, featureMessageType.ExcludedFeatures)
                : catalog.GetAllSids());
            for (var index = 0; index != allSids.Length; index++)
            {
                var user = ActorProxy.Create<IUser>(new ActorId(allSids[index]));
                await user.SendMessage(message);
            }
        }
    }
}
