namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class DirectTcpMessageHeader : MessageHeader
    {
        public override MessageHeaderType Type => MessageHeaderType.DirectTcp;

        public override string GetParametersText()
        {
            return string.Empty;
        }
    }
}
