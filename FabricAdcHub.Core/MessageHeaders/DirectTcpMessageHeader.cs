namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class DirectTcpMessageHeader : MessageHeader
    {
        public override MessageHeaderType Type => MessageHeaderType.DirectTcp;

        public override string ToText()
        {
            return string.Empty;
        }
    }
}
