using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.Messages;

namespace FabricAdcHub.Core.MessageTypes
{
    public sealed class FeatureBroadcastMessageType : MessageTypeWithSid
    {
        public FeatureBroadcastMessageType()
        {
            RequiredFeatures = new List<string>();
            ExcludedFeatures = new List<string>();
        }

        public override MessageTypeName MessageTypeName => MessageTypeName.FeatureBroadcast;

        public IList<string> RequiredFeatures { get; set; }

        public IList<string> ExcludedFeatures { get; set; }

        public override int FromText(IList<string> parameters)
        {
            Sid = parameters[0];
            var features = parameters[1];
            for (var index = 0; index < features.Length; index += 5)
            {
                var feature = features.Substring(index + 1, 4);
                if (features[index] == '+')
                {
                    RequiredFeatures.Add(feature);
                }
                else
                {
                    ExcludedFeatures.Add(feature);
                }
            }

            return 2;
        }

        public override string ToText()
        {
            var features =
                string.Join(string.Empty, RequiredFeatures.Select(feature => "+" + feature))
                + string.Join(string.Empty, ExcludedFeatures.Select(feature => "-" + feature));
            return Message.BuildString(Sid, features);
        }
    }
}
