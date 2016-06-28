using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class SupportsMessage : Message
    {
        public SupportsMessage(MessageType messageType)
            : this(messageType, Enumerable.Empty<string>(), Enumerable.Empty<string>())
        {
        }

        public SupportsMessage(MessageType messageType, IEnumerable<string> addFeatures, IEnumerable<string> removeFeatures)
            : base(messageType, MessageName.Supports)
        {
            AddFeatures = new List<string>(addFeatures);
            RemoveFeatures = new List<string>(removeFeatures);
        }

        public IEnumerable<string> AddFeatures { get; private set; }

        public IEnumerable<string> RemoveFeatures { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            var addFeatures = new List<string>();
            var removeFeatures = new List<string>();
            foreach (var parameter in parameters)
            {
                var operation = parameter.Substring(0, 2);
                var feature = parameter.Substring(2);
                if (operation == "AD")
                {
                    addFeatures.Add(feature);
                }
                else
                {
                    removeFeatures.Add(feature);
                }
            }

            AddFeatures = addFeatures;
            RemoveFeatures = removeFeatures;
        }

        protected override string GetParameters()
        {
            var add = BuildString(AddFeatures.Select(feature => "AD" + feature));
            var remove = BuildString(RemoveFeatures.Select(feature => "RM" + feature));
            return BuildString(add, remove);
        }
    }
}
