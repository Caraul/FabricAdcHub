using System.Collections.Generic;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class SidMessage : Message
    {
        public SidMessage(MessageType messageType)
            : base(messageType, MessageName.Sid)
        {
        }

        public SidMessage(MessageType messageType, string sid)
            : this(messageType)
        {
            Sid = sid;
        }

        public string Sid { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            Sid = parameters[0];
        }

        protected override string GetParameters()
        {
            return Sid;
        }
    }
}
