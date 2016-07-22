namespace FabricAdcHub.User.States
{
    internal class IdentifyState : StateBase
    {
        public IdentifyState(User user)
            : base(AdcProtocolState.Identify, user)
        {
        }
    }
}
