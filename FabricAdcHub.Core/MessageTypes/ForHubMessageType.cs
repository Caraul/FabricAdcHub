namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class ForHubMessageType : SimpleMessageType
    {
        public override MessageTypeName MessageTypeName
        {
            get { return MessageTypeName.ForHub; }
        }
    }
}
