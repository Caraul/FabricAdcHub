using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Supports : Command
    {
        public Supports(MessageHeader header, IList<string> parameters)
            : this(header)
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

        public Supports(MessageHeader header, IEnumerable<string> addFeatures, IEnumerable<string> removeFeatures)
            : this(header)
        {
            AddFeatures = new List<string>(addFeatures);
            RemoveFeatures = new List<string>(removeFeatures);
        }

        public IReadOnlyList<string> AddFeatures { get; }

        public IReadOnlyList<string> RemoveFeatures { get; }

        protected override string GetParametersText()
        {
            var add = BuildString(AddFeatures.Select(feature => "AD" + feature));
            var remove = BuildString(RemoveFeatures.Select(feature => "RM" + feature));
            return BuildString(add, remove);
        }

        private Supports(MessageHeader header)
            : base(header, CommandType.Supports)
        {
        }
    }
}
