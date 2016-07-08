using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.User.States
{
    internal class ProtocolState : StateBase
    {
        public ProtocolState(User user)
            : base(user)
        {
        }

        public override State State => State.Protocol;

        public override bool IsSendCommandAllowed(Command command)
        {
            return command.Type == CommandType.Status
                || command.Type == CommandType.Supports
                || command.Type == CommandType.Sid;
        }

        public override Task<State> ProcessCommand(Command command)
        {
            if (command.Type == CommandType.Supports)
            {
                return ProcessSupports((Supports)command);
            }

            if (command.Type == CommandType.Status)
            {
                return ProcessStatus((Status)command);
            }

            return ProcessInvalidCommand(command);
        }

        private async Task<State> ProcessSupports(Supports command)
        {
            var features = new[] { "BASE", "TIGR" };
            for (var index = 0; index != features.Length; index++)
            {
                var feature = features[index];
                if (!command.AddFeatures.Contains(feature))
                {
                    await SendRequiredFeatureIsMissing(feature);
                    return State;
                }
            }

            var supportsMessage = new Supports(InformationMessageHeader, features, Enumerable.Empty<string>());
            await User.SendCommand(supportsMessage);
            var sidMessage = new Sid(new InformationMessageHeader(), User.Sid);
            await User.SendCommand(sidMessage);
            return State.Identify;
        }

        private Task<State> ProcessStatus(Status command)
        {
            return Task.FromResult(command.Severity == Status.ErrorSeverity.Fatal ? State.DisconnectedOnProtocolError : State);
        }

        private Task SendRequiredFeatureIsMissing(string featureName)
        {
            var statusCommand = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.RequiredFeatureIsMissing,
                $"Feature {featureName} is required");
            return User.SendCommand(statusCommand);
        }

        private static readonly InformationMessageHeader InformationMessageHeader = new InformationMessageHeader();
    }
}
