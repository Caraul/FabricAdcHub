using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;

namespace FabricAdcHub.User.Transitions
{
    internal class ProtocolToIdentify : IfChoiceBase<AdcProtocolState, StateMachineEvent, Command>
    {
        public ProtocolToIdentify(User user)
            : base(
                new StateMachineEvent(InternalEvent.AdcMessageReceived, new Supports(new HubOnlyMessageHeader(), new List<string>())),
                AdcProtocolState.Identify,
                AdcProtocolState.Unknown)
        {
            _user = user;
        }

        public override Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            var command = (Supports) parameter;
            for (var index = 0; index != Features.Length; index++)
            {
                var feature = Features[index];
                if (!command.AddFeatures.Contains(feature))
                {
                    CreateRequiredFeatureIsMissing(feature);
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }

        public override async Task IfEffect(StateMachineEvent evt, Command parameter)
        {
            var supportsMessage = new Supports(InformationMessageHeader, Features, Enumerable.Empty<string>());
            await _user.SendCommand(supportsMessage);
            var sidMessage = new Sid(new InformationMessageHeader(), _user.Sid);
            await _user.SendCommand(sidMessage);
        }

        public override Task ElseEffect(StateMachineEvent evt, Command parameter)
        {
            return _user.SendCommand(_errorCommand);
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
        private readonly User _user;
        private Command _errorCommand;
    }
}
