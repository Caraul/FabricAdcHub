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
            var namedFlags = new NamedFlags(parameters.Skip(1));
            GroupSid = namedFlags.GetString("PM");
            AsMe = namedFlags.GetInt("ME");
        }

        public Msg(MessageHeader header, string text)
            : base(header, CommandType.Message)
        {
            Text = text;
        }

        public string Text { get; }

        public string GroupSid { get; set; }

        public int? AsMe { get; set; }

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags();
            namedFlags.SetString("PM", GroupSid);
            namedFlags.SetInt("ME", AsMe);
            return BuildString(Text, namedFlags.ToText());
        }
    }
}
