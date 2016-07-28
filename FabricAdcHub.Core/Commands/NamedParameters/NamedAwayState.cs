using System.Collections.Generic;
using System.Linq;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public sealed class NamedAwayState : NamedFlag<Information.AwayState>
    {
        public NamedAwayState(string name, NamedFlags namedFlags)
            : base(name, namedFlags)
        {
        }

        public override HashSet<string> ToStrings(Information.AwayState value)
        {
            return new HashSet<string>(new[] { value == Information.AwayState.Away ? "1" : "2" });
        }

        public override Information.AwayState FromStrings(HashSet<string> value)
        {
            return (Information.AwayState)int.Parse(value.First());
        }
    }
}
