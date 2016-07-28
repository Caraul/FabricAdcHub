using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.Commands;

namespace FabricAdcHub.Core.MessageHeaders
{
    public sealed class FeatureBroadcastMessageHeader : MessageHeaderWithSid
    {
        public FeatureBroadcastMessageHeader(string sid)
            : base(sid)
        {
            RequiredFeatures = new List<string>();
            ExcludedFeatures = new List<string>();
        }

        public FeatureBroadcastMessageHeader(IList<string> parameters)
            : base(parameters)
        {
            RequiredFeatures = new List<string>();
            ExcludedFeatures = new List<string>();
            FromText(parameters);
        }

        public override MessageHeaderType Type => MessageHeaderType.FeatureBroadcast;

        public IList<string> RequiredFeatures { get; }

        public IList<string> ExcludedFeatures { get; }

        public override string GetParametersText()
        {
            var features =
                string.Join(string.Empty, RequiredFeatures.Select(feature => "+" + feature))
                + string.Join(string.Empty, ExcludedFeatures.Select(feature => "-" + feature));
            return MessageSerializer.BuildText(Sid, features);
        }

        private void FromText(IList<string> parameters)
        {
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
        }
    }
}
