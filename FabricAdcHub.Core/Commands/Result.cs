using System.Collections.Generic;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Result : Command
    {
        public Result(MessageHeader header, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Result, namedParameters, originalMessage)
        {
        }

        public Result(MessageHeader header)
            : base(header, CommandType.Result)
        {
        }

        public NamedString FullFilename => GetString("FN");

        public NamedInt Size => GetInt("SI");

        public NamedInt AvailableSlots => GetInt("SL");

        public NamedString Token => GetString("TO");

        public NamedString Tth => GetString("TR");
    }
}
