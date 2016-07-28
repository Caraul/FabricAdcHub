using System.Collections.Generic;
using FabricAdcHub.Core.Commands;

namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class DirectMessageHeader : MessageHeaderWithSid
    {
        public DirectMessageHeader(IList<string> parameters)
            : base(parameters)
        {
            TargetSid = parameters[1];
        }

        public override MessageHeaderType Type => MessageHeaderType.Direct;

        public string TargetSid { get; set; }

        public override string GetParametersText()
        {
            return MessageSerializer.BuildText(Sid, TargetSid);
        }
    }
}
