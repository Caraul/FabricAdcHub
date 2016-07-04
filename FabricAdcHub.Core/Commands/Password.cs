using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Password : Command
    {
        public Password(MessageHeader header, IList<string> parameters)
            : this(header, parameters[0])
        {
        }

        public Password(MessageHeader header, string password)
            : base(header, CommandType.Password)
        {
            Hash = password;
        }

        public string Hash { get; }

        protected override string GetParametersText()
        {
            return Hash;
        }
    }
}
