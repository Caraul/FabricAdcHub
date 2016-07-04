using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Result : Command
    {
        public Result(MessageHeader header)
            : base(header, CommandType.Result)
        {
        }

        public Result(MessageHeader header, IList<string> parameters)
            : this(header)
        {
            var namedFlags = new NamedFlags(parameters);
            FullFilename = namedFlags.GetString("FN");
            Size = namedFlags.GetInt("SI");
            AvailableSlots = namedFlags.GetInt("SL");
            Token = namedFlags.GetString("TO");
            Tth = namedFlags.GetString("TR");
        }

        public string FullFilename { get; set; }

        public int? Size { get; set; }

        public int? AvailableSlots { get; set; }

        public string Token { get; set; }

        public string Tth { get; set; }

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags();
            namedFlags.SetString("FN", FullFilename);
            namedFlags.SetInt("SI", Size);
            namedFlags.SetInt("SL", AvailableSlots);
            namedFlags.SetString("TO", Token);
            namedFlags.SetString("TR", Tth);
            return BuildString(namedFlags.ToText());
        }
    }
}
