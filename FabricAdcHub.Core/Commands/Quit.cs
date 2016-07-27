using System;
using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Quit : Command
    {
        public Quit(MessageHeader header, IList<string> parameters)
            : this(header, parameters[0])
        {
            new NamedFlags(parameters.Skip(1))
                .Get(InitiatorSid)
                .Get(SecondsUntilReconnectIsAllowed)
                .Get(Message)
                .Get(RedirectTo)
                .Get(DisconnectAll);
        }

        public Quit(MessageHeader header, string sid)
            : base(header, CommandType.Quit)
        {
            Sid = sid;
        }

        public string Sid { get; }

        public NamedFlag<string> InitiatorSid { get; } = new NamedFlag<string>("ID");

        public NamedFlag<int> SecondsUntilReconnectIsAllowed { get; } = new NamedFlag<int>("TL");

        public NamedFlag<string> Message { get; } = new NamedFlag<string>("MS");

        public NamedFlag<Uri> RedirectTo { get; } = new NamedFlag<Uri>("RD");

        public NamedFlag<bool> DisconnectAll { get; } = new NamedFlag<bool>("DI");

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags()
                .Set(InitiatorSid)
                .Set(SecondsUntilReconnectIsAllowed)
                .Set(Message)
                .Set(RedirectTo)
                .Set(DisconnectAll);

            return BuildString(Sid, namedFlags.ToText());
        }
    }
}
