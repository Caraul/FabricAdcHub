namespace FabricAdcHub.Core.MessageHeaders
{
    public abstract class MessageHeader
    {
        public abstract MessageHeaderType Type { get; }

        public abstract string GetParametersText();
    }
}
