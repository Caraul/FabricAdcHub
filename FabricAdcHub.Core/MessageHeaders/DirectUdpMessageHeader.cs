using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class DirectUdpMessageHeader : MessageHeader
    {
        public DirectUdpMessageHeader(IList<string> parameters)
        {
            MyCid = parameters[0];
        }

        public override MessageHeaderType Type => MessageHeaderType.DirectUdp;

        public string MyCid { get; set; }

        public override string ToText()
        {
            return MyCid;
        }
    }
}
