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

        public virtual Task OnEnter()
        {
            return Task.CompletedTask;
        }

        public abstract bool IsSendCommandAllowed(Command command);

        public abstract Task<State> ProcessCommand(Command command);

        protected StateBase(User user)
        {
            User = user;
        }

        protected User User { get; }

        protected async Task<State> ProcessInvalidCommand(Command command)
        {
            var status = new Status(new InformationMessageHeader(), Status.ErrorSeverity.Fatal, Status.ErrorCode.InvalidState, $"Command {command.FourCc()} not available in {State} state.");
            status.OffendingCommandOrMissingFeature.Value = command.FourCc();
            await User.SendCommand(status);
            return State.DisconnectedOnProtocolError;
        }

        protected async Task BroadcastCommand(Command command)
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            await catalog.BroadcastMessage(User.Sid, command.ToMessage());
        }

        protected async Task DirectCommand(Command command)
        {
            var directMessageType = (DirectMessageHeader)command.Header;
            var user = ActorProxy.Create<IUser>(new ActorId(directMessageType.TargetSid));
            await user.SendMessage(command.ToMessage());
        }

        protected async Task FeatureBroadcastCommand(Command command)
        {
            var featureBroadcastMessageHeader = (FeatureBroadcastMessageHeader)command.Header;
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            await catalog.FeatureBroadcastMessage(User.Sid, featureBroadcastMessageHeader.RequiredFeatures.ToList(), featureBroadcastMessageHeader.ExcludedFeatures.ToList(), command.ToMessage());
        }
    }
}
