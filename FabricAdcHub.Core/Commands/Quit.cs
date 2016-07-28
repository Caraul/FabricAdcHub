using System.Collections.Generic;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Quit : Command
    {
        public Quit(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Quit, namedParameters, originalMessage)
        {
            Sid = positionalParameters[0];
        }

        public Quit(MessageHeader header, string sid)
            : base(header, CommandType.Quit)
        {
            Sid = sid;
        }

        public string Sid { get; }

        public NamedString InitiatorSid => GetString("ID");

        public NamedInt SecondsUntilReconnectIsAllowed => GetInt("TL");

        public NamedString Message => GetString("MS");

        public NamedUri RedirectTo => GetUri("RD");

        public NamedBool DisconnectAll => GetBool("DI");

        protected override string GetPositionalParametersText()
        {
            return Sid;
        }
    }
}
