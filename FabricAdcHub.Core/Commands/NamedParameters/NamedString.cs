using System.Collections.Generic;
using System.Linq;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedString : NamedFlag<string>
    {
        public NamedString(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(string value)
        {
            return new HashSet<string>(new[] { value });
        }

        public override string FromStrings(HashSet<string> value)
        {
            return value.First();
        }
    }
}
