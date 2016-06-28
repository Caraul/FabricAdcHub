using System.Collections.Generic;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class GetPasswordMessage : Message
    {
        public GetPasswordMessage(MessageType messageType)
            : base(messageType, MessageName.GetPassword)
        {
        }

        public GetPasswordMessage(MessageType messageType, string randomData)
            : this(messageType)
        {
            RandomData = randomData;
        }

        public string RandomData { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            RandomData = parameters[0];
        }

        protected override string GetParameters()
        {
            return RandomData;
        }
    }
}
