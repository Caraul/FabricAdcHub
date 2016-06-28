using System.Collections.Generic;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class SearchMessage : Message
    {
        public SearchMessage(MessageType messageType)
            : base(messageType, MessageName.Search)
        {
        }

        public IList<string> Include { get; private set; }

        public IList<string> Exclude { get; private set; }

        public IList<string> Extensions { get; private set; }

        public int? LessThanOrEqualSize { get; set; }

        public int? GreaterThanOrEqualSize { get; set; }

        public int? ExactSize { get; set; }

        public string Token { get; set; }

        public ItemType SearchItemType { get; set; }

        public string Tth { get; set; }

        public override void FromText(IList<string> parameters)
        {
            var namedParameters = new NamedParameters(parameters);
            Include = namedParameters.GetStrings("AN");
            Exclude = namedParameters.GetStrings("NO");
            Extensions = namedParameters.GetStrings("EX");
            LessThanOrEqualSize = namedParameters.GetInt("LE");
            GreaterThanOrEqualSize = namedParameters.GetInt("GE");
            ExactSize = namedParameters.GetInt("EQ");
            Token = namedParameters.GetString("TO");
            var itemType = namedParameters.GetNamedInt("TY");
            SearchItemType = itemType.IsUndefined ? ItemType.Any : (itemType.Value == 1 ? ItemType.File : ItemType.Directory);
            Tth = namedParameters.GetString("TD");
        }

        protected override string GetParameters()
        {
            var namedParameters = new NamedParameters();
            namedParameters.SetStrings("AN", Include);
            namedParameters.SetStrings("NO", Exclude);
            namedParameters.SetStrings("EX", Extensions);
            namedParameters.SetInt("LE", LessThanOrEqualSize);
            namedParameters.SetInt("GE", GreaterThanOrEqualSize);
            namedParameters.SetInt("EQ", ExactSize);
            namedParameters.SetString("TO", Token);
            namedParameters.SetInt("TY", SearchItemType == ItemType.Any ? (int?)null : (SearchItemType == ItemType.File ? 1 : 2));
            namedParameters.SetString("TD", Tth);
            return namedParameters.ToText();
        }

        public enum ItemType
        {
            Any = 0,
            File = 1,
            Directory = 2
        }
    }
}
