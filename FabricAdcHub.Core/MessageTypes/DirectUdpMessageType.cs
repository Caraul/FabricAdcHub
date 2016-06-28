using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class DirectUdpMessageType : MessageType
    {
        public override MessageTypeName MessageTypeName
        {
            get { return MessageTypeName.DirectUdp; }
        }

        public string MyCid { get; set; }

        public override int FromText(IList<string> parameters)
        {
            MyCid = parameters[0];
            return 1;
        }

        public override string ToText()
        {
            return MyCid;
        }
    }
}
