using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class BroadcastMessageHeader : MessageHeaderWithSid
    {
        public BroadcastMessageHeader(string sid)
            : base(sid)
        {
        }

        public BroadcastMessageHeader(IList<string> parameters)
            : base(parameters)
        {
        }

        public override MessageHeaderType Type => MessageHeaderType.Broadcast;

        public override string GetParametersText()
        {
            return Sid;
        }
    }
}
