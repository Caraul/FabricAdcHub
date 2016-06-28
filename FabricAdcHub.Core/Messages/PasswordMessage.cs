using System.Collections.Generic;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class PasswordMessage : Message
    {
        public PasswordMessage(MessageType messageType)
            : base(messageType, MessageName.Password)
        {
        }

        public PasswordMessage(MessageType messageType, string password)
            : this(messageType)
        {
            Password = password;
        }

        public string Password { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            Password = parameters[0];
        }

        protected override string GetParameters()
        {
            return Password;
        }
    }
}
