using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Sender.Interfaces;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace FabricAdcHub.User.Transitions
{
    internal abstract class AdcTransitionBase : TransitionBase<AdcProtocolState, StateMachineEvent, Command>
    {
        protected AdcTransitionBase(User user, AdcProtocolState destination, StateMachineEvent trigger)
            : base(destination, trigger)
        {
            User = user;
            Sender = ActorProxy.Create<ISender>(new ActorId(user.Sid));
        }

        public override Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            return Task.FromResult(true);
        }

        protected User User { get; }

        protected ISender Sender { get; }
    }
}
