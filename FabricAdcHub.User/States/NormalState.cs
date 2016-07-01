using System.Threading.Tasks;
using FabricAdcHub.Core.Messages;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.User.States
{
    internal class NormalState : StateBase
    {
        public NormalState(User user)
            : base(user)
        {
        }

        public override State State => State.Identify;

        public override async Task<State> ProcessMessage(Message message)
        {
            if (message.MessageName == MessageName.Information)
            {
                await User.UpdateInformation((InformationMessage)message);
            }

            if (message.MessageName == MessageName.Supports)
            {
                await User.UpdateInformation((SupportsMessage)message);
            }

            if (message.MessageType.MessageTypeName == MessageTypeName.Broadcast)
            {
                await BroadcastMessage(message);
                return State.Normal;
            }

            if (message.MessageType.MessageTypeName == MessageTypeName.Direct)
            {
                await DirectMessage(message);
                return State.Normal;
            }

            if (message.MessageType.MessageTypeName == MessageTypeName.Echo)
            {
                await DirectMessage(message);
                await User.SendMessage(message);
                return State.Normal;
            }

            if (message.MessageType.MessageTypeName == MessageTypeName.FeatureBroadcast)
            {
                await FeatureBroadcastMessage(message);
                return State.Normal;
            }

            return State.Normal;
        }
    }
}
