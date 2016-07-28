using FabricAdcHub.Core.Commands;
using FabricAdcHub.Sender.Interfaces;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace FabricAdcHub.User.Transitions
{
    internal abstract class AdcIfChoiceBase : IfChoiceBase<AdcProtocolState, StateMachineEvent, Command>
    {
        protected AdcIfChoiceBase(User user, StateMachineEvent trigger, AdcProtocolState ifDestination, AdcProtocolState elseDestination)
            : base(trigger, ifDestination, elseDestination)
        {
            User = user;
            Sender = ActorProxy.Create<ISender>(new ActorId(user.Sid));
        }

        protected User User { get; }

        protected ISender Sender { get; }
    }
}
