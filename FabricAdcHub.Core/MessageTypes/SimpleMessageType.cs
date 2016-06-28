using System.Collections.Generic;

namespace FabricAdcHub.Core.MessageTypes
{
    public abstract class SimpleMessageType : MessageType
    {
        public override int FromText(IList<string> parameters)
        {
            return 0;
        }

        public override string ToText()
        {
            return string.Empty;
        }
    }
}
