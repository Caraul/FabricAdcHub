using System.Globalization;
using System.Linq;

namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class MessageTypeName
    {
        public static readonly MessageTypeName Broadcast = new MessageTypeName('B');
        public static readonly MessageTypeName DirectFromHub = new MessageTypeName('D');
        public static readonly MessageTypeName Echo = new MessageTypeName('E');
        public static readonly MessageTypeName Information = new MessageTypeName('I');
        public static readonly MessageTypeName FeatureBroadcast = new MessageTypeName('F');
        public static readonly MessageTypeName ForHub = new MessageTypeName('H');
        public static readonly MessageTypeName DirectTcp = new MessageTypeName('C');
        public static readonly MessageTypeName DirectUdp = new MessageTypeName('U');

        public char MessageTypeSymbol { get; set; }

        public static MessageTypeName FromText(char text)
        {
            var validTypes = new[] { Broadcast, DirectFromHub, Echo, Information, FeatureBroadcast, ForHub, DirectTcp, DirectUdp };
            return validTypes.Single(validType => validType.MessageTypeSymbol == text);
        }

        public string ToText()
        {
            return MessageTypeSymbol.ToString(CultureInfo.InvariantCulture);
        }

        private MessageTypeName(char text)
        {
            MessageTypeSymbol = text;
        }
    }
}
