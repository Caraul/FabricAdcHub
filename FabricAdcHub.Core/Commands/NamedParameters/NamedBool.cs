using System.Collections.Generic;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedBool : NamedFlag<bool>
    {
        public NamedBool(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(bool value)
        {
            return new HashSet<string>(new[] { "1" });
        }

        public override bool FromStrings(HashSet<string> value)
        {
            return true;
        }
    }
}
