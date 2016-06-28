using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class MsgMessage : Message
    {
        public MsgMessage(MessageType messageType)
            : base(messageType, MessageName.Sid)
        {
        }

        public MsgMessage(MessageType messageType, string text)
            : this(messageType)
        {
            Text = text;
        }

        public string Text { get; private set; }

        public string GroupSid { get; set; }

        public int? AsMe { get; set; }

        public override void FromText(IList<string> parameters)
        {
            Text = parameters[0];
            var namedParameters = new NamedParameters(parameters.Skip(1));
            GroupSid = namedParameters.GetString("PM");
            AsMe = namedParameters.GetInt("ME");
        }

        protected override string GetParameters()
        {
            var namedParameters = new NamedParameters();
            namedParameters.SetString("PM", GroupSid);
            namedParameters.SetInt("ME", AsMe);
            return BuildString(Text, namedParameters.ToText());
        }
    }
}
