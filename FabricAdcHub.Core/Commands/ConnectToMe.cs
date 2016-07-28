using System.Collections.Generic;
using System.Globalization;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class ConnectToMe : Command
    {
        public ConnectToMe(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.ConnectToMe, namedParameters, originalMessage)
        {
            Protocol = positionalParameters[0];
            Port = int.Parse(positionalParameters[1]);
            Token = positionalParameters[2];
        }

        public ConnectToMe(MessageHeader header, string protocol, int port, string token)
            : base(header, CommandType.ConnectToMe)
        {
            Protocol = protocol;
            Port = port;
            Token = token;
        }

        public string Protocol { get; }

        public int Port { get; }

        public string Token { get; }

        protected override string GetPositionalParametersText()
        {
            return MessageSerializer.BuildText(Protocol, Port.ToString(CultureInfo.InvariantCulture), Token);
        }
    }
}
