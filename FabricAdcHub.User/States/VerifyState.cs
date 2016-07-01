using System.Threading.Tasks;
using FabricAdcHub.Core.Messages;

namespace FabricAdcHub.User.States
{
    internal class VerifyState : StateBase
    {
        public VerifyState(User user)
            : base(user)
        {
        }

        public override State State => State.Verify;

        public override Task<State> ProcessMessage(Message message)
        {
            return Task.FromResult(State);
        }
    }
}
