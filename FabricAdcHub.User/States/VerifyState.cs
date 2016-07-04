using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;

namespace FabricAdcHub.User.States
{
    internal class VerifyState : StateBase
    {
        public VerifyState(User user)
            : base(user)
        {
        }

        public override State State => State.Verify;

        public override Task<State> ProcessCommand(Command command)
        {
            return Task.FromResult(State);
        }
    }
}
