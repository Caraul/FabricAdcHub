using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.User.States
{
    internal class NormalState : StateBase
    {
        public NormalState(User user)
            : base(user)
        {
        }

        public override State State => State.Identify;

        public override bool IsSendCommandAllowed(Command command)
        {
            return true;
        }

        public override async Task<State> ProcessCommand(Command command)
        {
            if (command.Type == CommandType.Information)
            {
                await User.UpdateInformation((Information)command);
            }

            if (command.Type == CommandType.Supports)
            {
                await User.UpdateInformation((Supports)command);
            }

            if (command.Header is BroadcastMessageHeader)
            {
                await BroadcastCommand(command);
                return State.Normal;
            }

            if (command.Header is DirectTcpMessageHeader)
            {
                await DirectCommand(command);
                return State.Normal;
            }

            if (command.Header is EchoMessageHeader)
            {
                await DirectCommand(command);
                await User.SendCommand(command);
                return State.Normal;
            }

            if (command.Header is FeatureBroadcastMessageHeader)
            {
                await FeatureBroadcastCommand(command);
                return State.Normal;
            }

            return State.Normal;
        }
    }
}
