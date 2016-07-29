using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class EchoMessageHeader : MessageHeaderWithSid
    {
        public EchoMessageHeader(IList<string> parameters)
            : base(parameters)
        {
            TargetSid = parameters[1];
        }

        public override MessageHeaderType Type => MessageHeaderType.Echo;

        public string TargetSid { get; set; }

        public override string GetParametersText()
        {
            return MessageSerializer.BuildText(Sid, TargetSid);
        }
    }
}
