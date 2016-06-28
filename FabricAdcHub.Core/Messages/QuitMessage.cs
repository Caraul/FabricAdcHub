using System;
using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class QuitMessage : Message
    {
        public QuitMessage(MessageType messageType)
            : base(messageType, MessageName.Sid)
        {
        }

        public QuitMessage(MessageType messageType, string sid)
            : this(messageType)
        {
            Sid = sid;
        }

        public string Sid { get; private set; }

        public string InitiatorSid { get; set; }

        public int? SecondsUntilReconnectIsAllowed { get; set; }

        public string Message { get; set; }

        public Uri RedirectTo { get; set; }

        public bool? DisconnectAll { get; set; }

        public override void FromText(IList<string> parameters)
        {
            Sid = parameters[0];

            var namedParameters = new NamedParameters(parameters.Skip(1));
            InitiatorSid = namedParameters.GetString("ID");
            SecondsUntilReconnectIsAllowed = namedParameters.GetInt("TL");
            Message = namedParameters.GetString("MS");
            RedirectTo = namedParameters.GetValue("RD", value => new Uri(value));
            DisconnectAll = namedParameters.GetBool("DI");
        }

        protected override string GetParameters()
        {
            var namedParameters = new NamedParameters();
            namedParameters.SetString("ID", InitiatorSid);
            namedParameters.SetInt("TL", SecondsUntilReconnectIsAllowed);
            namedParameters.SetString("MS", Message);
            namedParameters.SetValue("RD", RedirectTo, value => value.ToString());
            namedParameters.SetBool("DI", DisconnectAll);

            return BuildString(Sid, namedParameters.ToText());
        }
    }
}
