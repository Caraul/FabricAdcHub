namespace FabricAdcHub.User.States
{
    internal enum State
    {
        Protocol,
        Identify,
        Verify,
        Normal,
        DisconnectedOnShutdown,
        DisconnectedOnNetworkError,
        DisconnectedOnProtocolError
    }
}
