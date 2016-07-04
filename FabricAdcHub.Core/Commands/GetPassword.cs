using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class GetPassword : Command
    {
        public GetPassword(MessageHeader header, IList<string> parameters)
            : this(header, parameters[0])
        {
        }

        public GetPassword(MessageHeader header, string randomData)
            : base(header, CommandType.GetPassword)
        {
            RandomData = randomData;
        }

        public string RandomData { get; }

        protected override string GetParametersText()
        {
            return RandomData;
        }
    }
}
