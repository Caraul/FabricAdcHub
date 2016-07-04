namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class InformationMessageHeader : MessageHeader
    {
        public override MessageHeaderType Type => MessageHeaderType.Information;

        public override string ToText()
        {
            return string.Empty;
        }
    }
}
