using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class ProtocolToUnknownTransition : AdcTransitionBase
    {
        public ProtocolToUnknownTransition(User user)
            : base(user, AdcProtocolState.Unknown, default(StateMachineEvent))
        {
        }

        public override async Task Effect(StateMachineEvent evt, Command parameter)
        {
            if (parameter != null)
            {
                var status = new Status(new InformationMessageHeader(), Status.ErrorSeverity.Fatal, Status.ErrorCode.InvalidState, $"Command {parameter.FourCc()} is invalid or not available in PROTOCOL state.");
                status.OffendingCommandOrMissingFeature.Value = parameter.FourCc();
                await Sender.SendMessage(status.ToMessage());
            }

            await User.Close();
        }
    }
}
