using System.Collections.Generic;
using System.Globalization;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class ConnectToMe : Command
    {
        public ConnectToMe(MessageHeader header, IList<string> parameters)
            : this(header, parameters[0], int.Parse(parameters[1]), parameters[2])
        {
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

        protected override string GetParametersText()
        {
            return BuildString(Protocol, Port.ToString(CultureInfo.InvariantCulture), Token);
        }
    }
}
