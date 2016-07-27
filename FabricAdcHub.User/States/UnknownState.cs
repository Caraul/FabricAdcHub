namespace FabricAdcHub.User.States
{
    internal class UnknownState : StateBase
    {
        public UnknownState(User user)
            : base(AdcProtocolState.Unknown, user)
        {
        }
    }
}
