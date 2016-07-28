namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class HubOnlyMessageHeader : MessageHeader
    {
        public override MessageHeaderType Type => MessageHeaderType.HubOnly;

        public override string GetParametersText()
        {
            return string.Empty;
        }
    }
}
