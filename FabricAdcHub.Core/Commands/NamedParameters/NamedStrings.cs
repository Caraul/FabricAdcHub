using System.Collections.Generic;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedStrings : NamedFlag<HashSet<string>>
    {
        public NamedStrings(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(HashSet<string> value)
        {
            return value;
        }

        public override HashSet<string> FromStrings(HashSet<string> value)
        {
            return value;
        }
    }
}
