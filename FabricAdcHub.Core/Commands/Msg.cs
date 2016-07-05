using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Msg : Command
    {
        public Msg(MessageHeader header, IList<string> parameters)
            : this(header, parameters[0])
        {
            new NamedFlags(parameters.Skip(1))
                .Get(GroupSid)
                .Get(AsMe);
        }

        public Msg(MessageHeader header, string text)
            : base(header, CommandType.Message)
        {
            Text = text;
        }

        public string Text { get; }

        public NamedFlag<string> GroupSid { get; } = new NamedFlag<string>("PM");

        public NamedFlag<int> AsMe { get; } = new NamedFlag<int>("ME");

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags()
                .Set(GroupSid)
                .Set(AsMe);
            return BuildString(Text, namedFlags.ToText());
        }
    }
}
