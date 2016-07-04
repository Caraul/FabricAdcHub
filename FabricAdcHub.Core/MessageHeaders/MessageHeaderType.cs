using System.Globalization;
using System.Linq;

namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class MessageHeaderType
    {
        public static readonly MessageHeaderType Broadcast = new MessageHeaderType('B', 1);
        public static readonly MessageHeaderType Direct = new MessageHeaderType('D', 2);
        public static readonly MessageHeaderType Echo = new MessageHeaderType('E', 2);
        public static readonly MessageHeaderType Information = new MessageHeaderType('I', 0);
        public static readonly MessageHeaderType FeatureBroadcast = new MessageHeaderType('F', 2);
        public static readonly MessageHeaderType HubOnly = new MessageHeaderType('H', 0);
        public static readonly MessageHeaderType DirectTcp = new MessageHeaderType('C', 0);
        public static readonly MessageHeaderType DirectUdp = new MessageHeaderType('U', 1);

        public char Symbol { get; }

        public int NumberOfParameters { get; }

        public static MessageHeaderType FromText(char text)
        {
            var validTypes = new[] { Broadcast, Direct, Echo, Information, FeatureBroadcast, HubOnly, DirectTcp, DirectUdp };
            return validTypes.Single(validType => validType.Symbol == text);
        }

        public string ToText()
        {
            return Symbol.ToString(CultureInfo.InvariantCulture);
        }

        private MessageHeaderType(char symbol, int numberOfParameters)
        {
            Symbol = symbol;
            NumberOfParameters = numberOfParameters;
        }
    }
}
