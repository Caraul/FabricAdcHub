using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class GetPassword : Command
    {
        public GetPassword(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.GetPassword, namedParameters, originalMessage)
        {
            RandomData = positionalParameters[0];
        }

        public GetPassword(MessageHeader header, string randomData)
            : base(header, CommandType.GetPassword)
        {
            RandomData = randomData;
        }

        public string RandomData { get; }

        protected override string GetPositionalParametersText()
        {
            return RandomData;
        }
    }
}
