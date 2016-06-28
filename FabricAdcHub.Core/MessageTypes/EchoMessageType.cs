using System.Collections.Generic;
using FabricAdcHub.Core.Messages;

namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class EchoMessageType : MessageTypeWithSid
    {
        public override MessageTypeName MessageTypeName
        {
            get { return MessageTypeName.Echo; }
        }

        public string TargetSid { get; set; }

        public override int FromText(IList<string> parameters)
        {
            Sid = parameters[0];
            TargetSid = parameters[1];
            return 2;
        }

        public override string ToText()
        {
            return Message.BuildString(Sid, TargetSid);
        }
    }
}
