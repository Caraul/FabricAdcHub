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

        public override Task<State> ProcessCommand(Command command)
        {
            if (command.Type == CommandType.Supports)
            {
                return ProcessCommand((Supports)command);
            }

            if (command.Type == CommandType.Status)
            {
                return ProcessCommand((Status)command);
            }

            throw new InvalidCommandException();
        }

        private async Task<State> ProcessCommand(Supports message)
        {
            var features = new[] { "BASE", "TIGR" };
            for (var index = 0; index != features.Length; index++)
            {
                var feature = features[index];
                if (!message.AddFeatures.Contains(feature))
                {
                    await SendRequiredFeatureIsMissing(feature);
                    return State;
                }
            }

            var supportsMessage = new Supports(InformationMessageType, features, Enumerable.Empty<string>());
            await User.SendMessage(supportsMessage);
            var sidMessage = new Sid(new InformationMessageHeader(), User.Id.GetStringId());
            await User.SendMessage(sidMessage);
            return State.Identify;
        }

        private Task<State> ProcessCommand(Status message)
        {
            return Task.FromResult(State);
        }

        private Task SendRequiredFeatureIsMissing(string featureName)
        {
            var statusMessage = new Status(
                InformationMessageType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.RequiredFeatureIsMissing,
                $"Feature {featureName} is required");
            return User.SendMessage(statusMessage);
        }

        private static readonly InformationMessageHeader InformationMessageType = new InformationMessageHeader();
    }
}
