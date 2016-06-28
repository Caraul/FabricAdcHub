using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class BroadcastMessageType : MessageTypeWithSid
    {
        public override MessageTypeName MessageTypeName
        {
            get { return MessageTypeName.Broadcast; }
        }

        public override int FromText(IList<string> parameters)
        {
            Sid = parameters[0];
            return 1;
        }

        public override string ToText()
        {
            return Sid;
        }
    }
}
