namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class DirectTcpMessageType : SimpleMessageType
    {
        public override MessageTypeName MessageTypeName => MessageTypeName.DirectTcp;
    }
}
