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
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            await catalog.BroadcastCommand(User.Sid, command);
        }

        protected async Task DirectCommand(Command command)
        {
            var directMessageType = (DirectMessageHeader)command.Header;
            var user = ActorProxy.Create<IUser>(new ActorId(directMessageType.TargetSid));
            await user.SendCommand(command);
        }

        protected async Task FeatureBroadcastCommand(Command command)
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            await catalog.FeatureBroadcastCommand(User.Sid, command);
        }
    }
}
