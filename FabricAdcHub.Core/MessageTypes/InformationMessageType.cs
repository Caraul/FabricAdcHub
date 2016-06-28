namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class InformationMessageType : SimpleMessageType
    {
        public override MessageTypeName MessageTypeName
        {
            get { return MessageTypeName.Information; }
        }
    }
}
