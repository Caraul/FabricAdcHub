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
            new NamedFlags(parameters)
                .Get(FullFilename)
                .Get(Size)
                .Get(AvailableSlots)
                .Get(Token)
                .Get(Tth);
        }

        public NamedFlag<string> FullFilename { get; } = new NamedFlag<string>("FN");

        public NamedFlag<int> Size { get; } = new NamedFlag<int>("SI");

        public NamedFlag<int> AvailableSlots { get; } = new NamedFlag<int>("SL");

        public NamedFlag<string> Token { get; } = new NamedFlag<string>("TO");

        public NamedFlag<string> Tth { get; } = new NamedFlag<string>("TR");

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags()
                .Set(FullFilename)
                .Set(Size)
                .Set(AvailableSlots)
                .Set(Token)
                .Set(Tth);
            return BuildString(namedFlags.ToText());
        }
    }
}
