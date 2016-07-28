using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class ReversedConnectToMe : Command
    {
        public ReversedConnectToMe(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.ReversedConnectToMe, namedParameters, originalMessage)
        {
            Protocol = positionalParameters[0];
            Token = positionalParameters[1];
        }

        public ReversedConnectToMe(MessageHeader header, string protocol, string token)
            : base(header, CommandType.ReversedConnectToMe)
        {
            Protocol = protocol;
            Token = token;
        }

        public string Protocol { get; }

        public string Token { get; }

        protected override string GetPositionalParametersText()
        {
            return MessageSerializer.BuildText(Protocol, Token);
        }
    }
}
