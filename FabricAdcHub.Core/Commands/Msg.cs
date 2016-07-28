using System.Collections.Generic;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Msg : Command
    {
        public Msg(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Message, namedParameters, originalMessage)
        {
            Text = positionalParameters[0];
        }

        public Msg(MessageHeader header, string text)
            : base(header, CommandType.Message)
        {
            Text = text;
        }

        public string Text { get; }

        public NamedString GroupSid => GetString("PM");

        public NamedInt AsMe => GetInt("ME");

        protected override string GetPositionalParametersText()
        {
            return Text;
        }
    }
}
