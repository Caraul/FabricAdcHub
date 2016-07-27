namespace FabricAdcHub.User.States
{
    internal enum AdcProtocolState
    {
        Unknown,
        Protocol,
        Identify,
        Normal,
        DisconnectedOnShutdown,
        DisconnectedOnNetworkError,
        DisconnectedOnProtocolError
    }
}
