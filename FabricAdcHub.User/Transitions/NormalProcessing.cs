using System;
using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Sender.Interfaces;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.States;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.User.Transitions
{
    internal class NormalProcessing : AdcTransitionBase
    {
        public NormalProcessing(User user)
            : base(user, AdcProtocolState.Normal, default(StateMachineEvent))
        {
        }

        public override async Task Effect(StateMachineEvent evt, Command parameter)
        {
            if (evt.InternalEvent == InternalEvent.ConnectionTimedOut)
            {
                // do nothing
                return;
            }

            if (parameter.Type == CommandType.Information)
            {
                await User.UpdateInformation((Information)parameter);
            }
            else if (parameter.Type == CommandType.Supports)
            {
                await User.UpdateInformation((Supports)parameter);
            }

            if (parameter.Header is BroadcastMessageHeader)
            {
                await BroadcastCommand(parameter);
                return;
            }

            if (parameter.Header is DirectMessageHeader)
            {
                await DirectCommand(parameter);
                return;
            }

            if (parameter.Header is EchoMessageHeader)
            {
                await EchoedDirectCommand(parameter);
                return;
            }

            if (parameter.Header is FeatureBroadcastMessageHeader)
            {
                await FeatureBroadcastCommand(parameter);
            }
        }

        protected async Task BroadcastCommand(Command command)
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            await catalog.BroadcastMessage(User.Sid, command.OriginalMessage);
        }

        protected async Task DirectCommand(Command command)
        {
            var directMessageType = (DirectMessageHeader)command.Header;
            var sender = ActorProxy.Create<ISender>(new ActorId(directMessageType.TargetSid));
            await sender.SendMessage(command.OriginalMessage);
        }

        protected async Task EchoedDirectCommand(Command command)
        {
            var echoMessageType = (EchoMessageHeader)command.Header;
            var sender = ActorProxy.Create<ISender>(new ActorId(echoMessageType.TargetSid));
            await sender.SendMessage(command.OriginalMessage);
            await Sender.SendMessage(command.OriginalMessage);
        }

        protected async Task FeatureBroadcastCommand(Command command)
        {
            var featureBroadcastMessageHeader = (FeatureBroadcastMessageHeader)command.Header;
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"), targetReplicaSelector: TargetReplicaSelector.RandomReplica);
            await catalog.FeatureBroadcastMessage(User.Sid, featureBroadcastMessageHeader.RequiredFeatures.ToList(), featureBroadcastMessageHeader.ExcludedFeatures.ToList(), command.ToMessage());
        }
    }
}
