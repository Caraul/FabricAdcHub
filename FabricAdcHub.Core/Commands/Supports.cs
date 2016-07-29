using System.Collections.Generic;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Supports : Command
    {
        public Supports(MessageHeader header, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Supports, namedParameters, originalMessage)
        {
        }

        public Supports(MessageHeader header, HashSet<string> addFeatures, HashSet<string> removeFeatures)
            : base(header, CommandType.Supports)
        {
            AddFeatures.Value = addFeatures;
            RemoveFeatures.Value = removeFeatures;
        }

        public NamedStrings AddFeatures => GetStrings("AD");

        public NamedStrings RemoveFeatures => GetStrings("RM");
    }
}
