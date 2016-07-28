using System.Collections.Generic;
using System.Linq;
using System.Text;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public abstract class Command
    {
        protected Command(MessageHeader header, CommandType type)
            : this(header, type, new List<string>(), string.Empty)
        {
        }

        protected Command(MessageHeader header, CommandType type, IList<string> namedParameters, string originalMessage)
        {
            Header = header;
            Type = type;
            NamedFlags = new NamedFlags(namedParameters);
            OriginalMessage = originalMessage;
        }

        public MessageHeader Header { get; }

        public CommandType Type { get; }

        public NamedFlags NamedFlags { get; }

        public string OriginalMessage { get; }

        public string FourCc()
        {
            return $"{Header.Type.Symbol}{Type.ToText()}";
        }

        public string ToMessage()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Header.GetParametersText());
            var positionalParametersText = GetPositionalParametersText();
            if (!string.IsNullOrEmpty(positionalParametersText))
            {
                stringBuilder.Append($"{MessageSerializer.Separator}{positionalParametersText}");
            }

            var namedParametersText = NamedFlags.ToText();
            if (!string.IsNullOrEmpty(namedParametersText))
            {
                stringBuilder.Append($"{MessageSerializer.Separator}{namedParametersText}");
            }

            return $"{Header.Type.Symbol}{Type.ToText()}{MessageSerializer.Separator}{stringBuilder}";
        }

        protected virtual string GetPositionalParametersText()
        {
            return string.Empty;
        }

        protected NamedBool GetBool(string name)
        {
            return new NamedBool(name, NamedFlags);
        }

        protected NamedInt GetInt(string name)
        {
            return new NamedInt(name, NamedFlags);
        }

        protected NamedString GetString(string name)
        {
            return new NamedString(name, NamedFlags);
        }

        protected NamedUri GetUri(string name)
        {
            return new NamedUri(name, NamedFlags);
        }

        protected NamedStrings GetStrings(string name)
        {
            return new NamedStrings(name, NamedFlags);
        }
    }
}
