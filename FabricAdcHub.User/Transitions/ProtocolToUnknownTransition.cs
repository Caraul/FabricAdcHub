using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Sender.Interfaces;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace FabricAdcHub.User.Transitions
{
    internal class ProtocolToUnknownTransition : TransitionBase<AdcProtocolState, StateMachineEvent, Command>
    {
        public ProtocolToUnknownTransition(User user)
            : base(AdcProtocolState.Unknown, default(StateMachineEvent))
        {
            _user = user;
            _sender = ActorProxy.Create<ISender>(new ActorId(_user.Sid));
        }

        public override Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            return Task.FromResult(true);
        }

        public override async Task Effect(StateMachineEvent evt, Command parameter)
        {
            if (parameter != null)
            {
                var status = new Status(new InformationMessageHeader(), Status.ErrorSeverity.Fatal, Status.ErrorCode.InvalidState, $"Command {parameter.FourCc()} is invalid or not available in PROTOCOL state.");
                status.OffendingCommandOrMissingFeature.Value = parameter.FourCc();
                await _sender.SendMessage(status.ToMessage());
            }

            await _user.Close();
        }

        private readonly User _user;
        private readonly ISender _sender;
    }
}
