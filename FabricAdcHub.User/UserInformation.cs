using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;

namespace FabricAdcHub.User
{
    public class UserInformation : NamedFlags
    {
        public UserInformation(string text)
            : base(MessageSerializer.SplitText(text))
        {
        }

        public HashSet<string> Features
        {
            get
            {
                var namedFeatures = new NamedFeatures("SU", this);
                return namedFeatures.Value;
            }
        }

        public void UpdateFromCommand(Information message)
        {
            Union(message.NamedFlags);
            Summarize();
        }

        public void UpdateFromCommand(Supports message)
        {
            var namedFeatures = new NamedFeatures("SU", this);
            var features = namedFeatures.Value;
            features.ExceptWith(message.RemoveFeatures.Value);
            features.UnionWith(message.AddFeatures.Value);
            namedFeatures.Value = features;
        }

        public Information ToCommand(MessageHeader messageType)
        {
            return new Information(
                messageType,
                Flags.Where(pair => pair.Key != "PD").SelectMany(pair => pair.Value.Select(value => (pair.Key + value).Escape())).ToList(),
                string.Empty);
        }
    }
}
