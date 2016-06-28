using System.Collections.Generic;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class ReverseConnectToMeMessage : Message
    {
        public ReverseConnectToMeMessage(MessageType messageType)
            : base(messageType, MessageName.ReversedConnectToMe)
        {
        }

        public ReverseConnectToMeMessage(MessageType messageType, string protocol, string token)
            : this(messageType)
        {
            Protocol = protocol;
            Token = token;
        }

        public string Protocol { get; private set; }

        public string Token { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            Protocol = parameters[0];
            Token = parameters[1];
        }

        protected override string GetParameters()
        {
            return BuildString(Protocol, Token);
        }
    }
}
