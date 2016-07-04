using System.Collections.Generic;
using FabricAdcHub.Core.Commands;

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

        public override string ToText()
        {
            return Command.BuildString(Sid, TargetSid);
        }
    }
}
