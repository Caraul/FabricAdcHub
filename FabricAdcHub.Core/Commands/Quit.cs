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
            var namedFlags = new NamedFlags(parameters.Skip(1));
            InitiatorSid = namedFlags.GetString("ID");
            SecondsUntilReconnectIsAllowed = namedFlags.GetInt("TL");
            Message = namedFlags.GetString("MS");
            RedirectTo = namedFlags.GetValue("RD", value => new Uri(value));
            DisconnectAll = namedFlags.GetBool("DI");
        }

        public Quit(MessageHeader header, string sid)
            : base(header, CommandType.Sid)
        {
            Sid = sid;
        }

        public string Sid { get; }

        public string InitiatorSid { get; set; }

        public int? SecondsUntilReconnectIsAllowed { get; set; }

        public string Message { get; set; }

        public Uri RedirectTo { get; set; }

        public bool? DisconnectAll { get; set; }

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags();
            namedFlags.SetString("ID", InitiatorSid);
            namedFlags.SetInt("TL", SecondsUntilReconnectIsAllowed);
            namedFlags.SetString("MS", Message);
            namedFlags.SetValue("RD", RedirectTo, value => value.ToString());
            namedFlags.SetBool("DI", DisconnectAll);

            return BuildString(Sid, namedFlags.ToText());
        }
    }
}
