using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageHeaders
{
    public abstract class MessageHeaderWithSid : MessageHeader
    {
        public string Sid { get; }

        protected MessageHeaderWithSid(string sid)
        {
            Sid = sid;
        }

        protected MessageHeaderWithSid(IList<string> parameters)
        {
            Sid = parameters[0];
        }
    }
}
