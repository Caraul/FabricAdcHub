using System.Collections.Generic;
using System.Linq;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedItemType : NamedFlag<Search.ItemType>
    {
        public NamedItemType(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(Search.ItemType value)
        {
            return new HashSet<string>(new[] { value == Search.ItemType.File ? "1" : "2" });
        }

        public override Search.ItemType FromStrings(HashSet<string> value)
        {
            return value.First() == "1" ? Search.ItemType.File : Search.ItemType.Directory;
        }
    }
}
