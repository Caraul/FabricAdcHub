using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class ProtocolStatusChoice : IfChoiceBase<AdcProtocolState, StateMachineEvent, Command>
    {
        public ProtocolStatusChoice(User user)
            : base(
                new StateMachineEvent(InternalEvent.AdcMessageReceived, CommandType.Status),
                AdcProtocolState.Protocol,
                AdcProtocolState.Unknown)
        {
            _user = user;
        }

        public override Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            var status = (Status)parameter;
            return Task.FromResult(status.Severity == Status.ErrorSeverity.Fatal);
        }

        public override Task IfEffect(StateMachineEvent evt, Command parameter)
        {
            return Task.CompletedTask;
        }

        public override Task ElseEffect(StateMachineEvent evt, Command parameter)
        {
            return _user.Close();
        }

        private readonly User _user;
    }
}
