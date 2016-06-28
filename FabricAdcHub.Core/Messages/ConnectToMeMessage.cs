using System.Collections.Generic;
using System.Globalization;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class ConnectToMeMessage : Message
    {
        public ConnectToMeMessage(MessageType messageType)
            : base(messageType, MessageName.ConnectToMe)
        {
        }

        public ConnectToMeMessage(MessageType messageType, string protocol, int port, string token)
            : this(messageType)
        {
            Protocol = protocol;
            Port = port;
            Token = token;
        }

        public string Protocol { get; private set; }

        public int Port { get; private set; }

        public string Token { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            Protocol = parameters[0];
            Port = int.Parse(parameters[1]);
            Token = parameters[2];
        }

        protected override string GetParameters()
        {
            return BuildString(Protocol, Port.ToString(CultureInfo.InvariantCulture), Token);
        }
    }
}
