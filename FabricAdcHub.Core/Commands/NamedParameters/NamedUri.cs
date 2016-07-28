using System;
using System.Collections.Generic;
using System.Linq;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedUri : NamedFlag<Uri>
    {
        public NamedUri(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(Uri value)
        {
            return new HashSet<string>(new[] { value.ToString() });
        }

        public override Uri FromStrings(HashSet<string> value)
        {
            return new Uri(value.First());
        }
    }
}
