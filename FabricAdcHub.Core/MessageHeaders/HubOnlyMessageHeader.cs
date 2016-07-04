namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class HubOnlyMessageHeader : MessageHeader
    {
        public override MessageHeaderType Type => MessageHeaderType.HubOnly;

        public override string ToText()
        {
            return string.Empty;
        }
    }
}
