using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class NormalToUnknownTransition : TransitionBase<AdcProtocolState, StateMachineEvent, Command>
    {
        public NormalToUnknownTransition(User user)
            : base(AdcProtocolState.Unknown, StateMachineEvent.DisconnectOccured)
        {
            _user = user;
        }

        public override Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            return Task.FromResult(true);
        }

        public override async Task Effect(StateMachineEvent evt, Command parameter)
        {
            await _user.Close();
        }

        private readonly User _user;
    }
}
