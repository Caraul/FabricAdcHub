using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public abstract class Command
    {
        protected Command(MessageHeader header, CommandType type)
        {
            Header = header;
            Type = type;
        }

        public MessageHeader Header { get; }

        public CommandType Type { get; }

        public string FourCc()
        {
            return $"{Header.Type.Symbol}{Type.ToText()}";
        }

        public virtual string ToText()
        {
            var allParameters = $"{Header.ToText()}{MessageSerializer.Separator}{GetParametersText()}".Trim();
            return $"{Header.Type.Symbol}{Type.ToText()}{MessageSerializer.Separator}{allParameters}";
        }

        public static string BuildString(IEnumerable<string> parts)
        {
            return BuildString(parts.ToArray());
        }

        public static string BuildString(params string[] parts)
        {
            return string.Join(MessageSerializer.Separator, parts);
        }

        protected abstract string GetParametersText();
    }
}
