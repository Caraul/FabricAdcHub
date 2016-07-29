using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class SupportsChoice : AdcIfChoiceBase
    {
        public SupportsChoice(User user)
            : base(
                user,
                new StateMachineEvent(InternalEvent.AdcMessageReceived, CommandType.Supports),
                AdcProtocolState.Identify,
                AdcProtocolState.Unknown)
        {
        }

        public override Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            var command = (Supports)parameter;
            for (var index = 0; index != Features.Length; index++)
            {
                var feature = Features[index];
                if (!command.AddFeatures.Value.Contains(feature))
                {
                    CreateRequiredFeatureIsMissing(feature);
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }

        public override async Task IfEffect(StateMachineEvent evt, Command parameter)
        {
            var supportsMessage = new Supports(InformationMessageHeader, new HashSet<string>(Features), null);
            await Sender.SendMessage(supportsMessage.ToMessage());
            var sidMessage = new Sid(new InformationMessageHeader(), User.Sid);
            await Sender.SendMessage(sidMessage.ToMessage());
        }

        public override Task ElseEffect(StateMachineEvent evt, Command parameter)
        {
            return Sender.SendMessage(_errorCommand.ToMessage());
        }

        private void CreateRequiredFeatureIsMissing(string featureName)
        {
            _errorCommand = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.RequiredFeatureIsMissing,
                $"Feature {featureName} is required");
        }

        private static readonly string[] Features = { "BASE", "TIGR" };
        private static readonly InformationMessageHeader InformationMessageHeader = new InformationMessageHeader();
        private Command _errorCommand;
    }
}
