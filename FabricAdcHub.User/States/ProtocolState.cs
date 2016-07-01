using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Core.Messages;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.User.States
{
    internal class ProtocolState : StateBase
    {
        public ProtocolState(User user)
            : base(user)
        {
        }

        public override State State => State.Protocol;

        public override Task<State> ProcessMessage(Message message)
        {
            if (message.MessageName == MessageName.Supports)
            {
                return ProcessMessage((SupportsMessage)message);
            }

            if (message.MessageName == MessageName.Status)
            {
                return ProcessMessage((StatusMessage)message);
            }

            throw new InvalidCommandException();
        }

        private async Task<State> ProcessMessage(SupportsMessage message)
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

            var supportsMessage = new SupportsMessage(InformationMessageType, features, Enumerable.Empty<string>());
            await User.SendMessage(supportsMessage);
            var sidMessage = new SidMessage(new InformationMessageType(), User.Id.GetStringId());
            await User.SendMessage(sidMessage);
            return State.Identify;
        }

        private Task<State> ProcessMessage(StatusMessage message)
        {
            return Task.FromResult(State);
        }

        private Task SendRequiredFeatureIsMissing(string featureName)
        {
            var statusMessage = new StatusMessage(
                InformationMessageType,
                StatusMessage.ErrorSeverity.Fatal,
                StatusMessage.ErrorCode.RequiredFeatureIsMissing,
                $"Feature {featureName} is required");
            return User.SendMessage(statusMessage);
        }

        private static readonly InformationMessageType InformationMessageType = new InformationMessageType();
    }
}
