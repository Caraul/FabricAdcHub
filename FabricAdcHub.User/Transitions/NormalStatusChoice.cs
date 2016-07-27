using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class NormalStatusChoice : AdcIfChoiceBase
    {
        public NormalStatusChoice(User user)
            : base(
                user,
                new StateMachineEvent(InternalEvent.AdcMessageReceived, CommandType.Status),
                AdcProtocolState.Normal,
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

        public override async Task ElseEffect(StateMachineEvent evt, Command parameter)
        {
            var quit = new Quit(new InformationMessageHeader(), _user.Sid);
            await Sender.SendMessage(quit.ToMessage());
            await _user.Close();
        }

        private readonly User _user;
    }
}
