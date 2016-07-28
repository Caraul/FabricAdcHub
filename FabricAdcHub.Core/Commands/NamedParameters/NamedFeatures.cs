using System;
using System.Collections.Generic;
using System.Linq;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedFeatures : NamedFlag<HashSet<string>>
    {
        public NamedFeatures(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(HashSet<string> value)
        {
            return new HashSet<string>(new[] { string.Join(",", value) });
        }

        public override HashSet<string> FromStrings(HashSet<string> value)
        {
            return new HashSet<string>(value.First().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
