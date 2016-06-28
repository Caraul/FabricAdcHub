using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public abstract class Message
    {
        protected Message(MessageType messageType, MessageName messageName)
        {
            MessageType = messageType;
            MessageName = messageName;
        }

        public MessageType MessageType { get; set; }

        public MessageName MessageName { get; set; }

        public abstract void FromText(IList<string> parameters);

        public virtual string ToText()
        {
            var allParameters = string.Format("{1}{0}{2}", MessageSerializer.Separator, MessageType.ToText(), GetParameters()).Trim();
            return string.Format(
                "{1}{2}{0}{3}",
                MessageSerializer.Separator,
                MessageType.MessageTypeName.MessageTypeSymbol,
                MessageName.ToText(),
                allParameters);
        }

        public static string BuildString(IEnumerable<string> parts)
        {
            return BuildString(parts.ToArray());
        }

        public static string BuildString(params string[] parts)
        {
            return string.Join(MessageSerializer.Separator, parts);
        }

        protected abstract string GetParameters();
    }
}
