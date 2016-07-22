using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery;
using FabricAdcHub.User.Machinery.ObjectOriented;

namespace FabricAdcHub.User.States
{
    internal abstract class StateBase : StateBase<AdcProtocolState, StateMachineEvent, Command>
    {
        public override Task OnEntry(Transition<AdcProtocolState, StateMachineEvent, Command> transition)
        {
            ActorEventSource.Current.StateEntered(User.Sid, State.ToString());

            return Task.CompletedTask;
        }

        public override Task OnExit(Transition<AdcProtocolState, StateMachineEvent, Command> transition)
        {
            ActorEventSource.Current.StateExited(User.Sid, State.ToString());

            return Task.CompletedTask;
        }

        protected StateBase(AdcProtocolState state, User user)
            : base(state)
        {
            User = user;
        }

        protected User User { get; }
    }
}
