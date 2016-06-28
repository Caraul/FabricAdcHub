namespace FabricAdcHub.Core.MessageTypes
{
    public abstract class MessageTypeWithSid : MessageType
    {
        public string Sid { get; set; }
    }
}
