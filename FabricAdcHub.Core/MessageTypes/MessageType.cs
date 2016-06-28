using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageTypes
{
    public abstract class MessageType
    {
        public abstract MessageTypeName MessageTypeName { get; }

        public abstract int FromText(IList<string> parameters);

        public abstract string ToText();
    }
}
