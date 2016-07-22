using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class ToUnknownTransition : TransitionBase<AdcProtocolState, StateMachineEvent, Command>
    {
        public ToUnknownTransition(User user)
            : base(AdcProtocolState.Unknown, default(StateMachineEvent))
        {
            _user = user;
        }

        public override Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            return Task.FromResult(true);
        }

        public override Task Effect(StateMachineEvent evt, Command parameter)
        {
            _user.Close();
            return Task.CompletedTask;
        }

        private readonly User _user;
    }
}
