using System.Collections.Generic;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class ResultMessage : Message
    {
        public ResultMessage(MessageType messageType)
            : base(messageType, MessageName.Result)
        {
        }

        public string FullFilename { get; set; }

        public int? Size { get; set; }

        public int? AvailableSlots { get; set; }

        public string Token { get; set; }

        public string Tth { get; set; }

        public override void FromText(IList<string> parameters)
        {
            var namedParameters = new NamedParameters(parameters);
            FullFilename = namedParameters.GetString("FN");
            Size = namedParameters.GetInt("SI");
            AvailableSlots = namedParameters.GetInt("SL");
            Token = namedParameters.GetString("TO");
            Tth = namedParameters.GetString("TR");
        }

        protected override string GetParameters()
        {
            var namedParameters = new NamedParameters();
            namedParameters.SetString("FN", FullFilename);
            namedParameters.SetInt("SI", Size);
            namedParameters.SetInt("SL", AvailableSlots);
            namedParameters.SetString("TO", Token);
            namedParameters.SetString("TR", Tth);
            return BuildString(namedParameters.ToText());
        }
    }
}
