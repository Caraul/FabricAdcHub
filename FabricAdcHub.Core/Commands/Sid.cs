using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Sid : Command
    {
        public Sid(MessageHeader header, IList<string> parameters)
            : this(header, parameters[0])
        {
        }

        public Sid(MessageHeader header, string sid)
            : base(header, CommandType.Sid)
        {
            Identifier = sid;
        }

        public string Identifier { get; }

        protected override string GetParametersText()
        {
            return Identifier;
        }
    }
}
