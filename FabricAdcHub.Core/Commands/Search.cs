using System.Collections.Generic;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Search : Command
    {
        public Search(MessageHeader header, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Search, namedParameters, originalMessage)
        {
        }

        public Search(MessageHeader header)
            : base(header, CommandType.Search)
        {
        }

        public NamedStrings Include => GetStrings("AN");

        public NamedStrings Exclude => GetStrings("NO");

        public NamedStrings Extensions => GetStrings("EX");

        public NamedInt LessThanOrEqualSize => GetInt("LE");

        public NamedInt GreaterThanOrEqualSize => GetInt("GE");

        public NamedInt ExactSize => GetInt("EQ");

        public NamedString Token => GetString("TO");

        public NamedItemType SearchItemType => new NamedItemType("TY", NamedFlags);

        public NamedString Tth => GetString("TD");

        public enum ItemType
        {
            File = 1,
            Directory = 2
        }
    }
}
