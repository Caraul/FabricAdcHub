using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class ReversedConnectToMe : Command
    {
        public ReversedConnectToMe(MessageHeader header, IList<string> parameters)
            : this(header, parameters[0], parameters[1])
        {
        }

        public ReversedConnectToMe(MessageHeader header, string protocol, string token)
            : base(header, CommandType.ReversedConnectToMe)
        {
            Protocol = protocol;
            Token = token;
        }

        public string Protocol { get; }

        public string Token { get; }

        protected override string GetParametersText()
        {
            return BuildString(Protocol, Token);
        }
    }
}
