using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedClientTypes : NamedFlag<Information.ClientTypes>
    {
        public NamedClientTypes(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(Information.ClientTypes value)
        {
            return new HashSet<string>(new[] { ((int)value).ToString(CultureInfo.InvariantCulture) });
        }

        public override Information.ClientTypes FromStrings(HashSet<string> value)
        {
            return (Information.ClientTypes)int.Parse(value.First());
        }
    }
}
