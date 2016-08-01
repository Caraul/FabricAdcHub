using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class NormalToUnknownTransition : AdcTransitionBase
    {
        public NormalToUnknownTransition(User user)
            : base(user, AdcProtocolState.Unknown, StateMachineEvent.DisconnectOccured)
        {
        }

        public override async Task Effect(StateMachineEvent evt, Command parameter)
        {
            await User.Close();
        }
    }
}
