namespace FabricAdcHub.User.States
{
    internal class ProtocolState : StateBase
    {
        public ProtocolState(User user)
            : base(AdcProtocolState.Protocol, user)
        {
        }
    }
}
