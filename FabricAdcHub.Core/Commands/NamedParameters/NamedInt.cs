using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedInt : NamedFlag<int>
    {
        public NamedInt(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(int value)
        {
            return new HashSet<string>(new[] { value.ToString(CultureInfo.InvariantCulture) });
        }

        public override int FromStrings(HashSet<string> value)
        {
            return int.Parse(value.First());
        }
    }
}
