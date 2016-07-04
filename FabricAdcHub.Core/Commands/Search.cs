using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Search : Command
    {
        public Search(MessageHeader header)
            : base(header, CommandType.Search)
        {
            Include = new List<string>();
            Exclude = new List<string>();
            Extensions = new List<string>();
        }

        public Search(MessageHeader header, IList<string> parameters)
            : this(header)
        {
            var namedFlags = new NamedFlags(parameters);
            Include = namedFlags.GetStrings("AN");
            Exclude = namedFlags.GetStrings("NO");
            Extensions = namedFlags.GetStrings("EX");
            LessThanOrEqualSize = namedFlags.GetInt("LE");
            GreaterThanOrEqualSize = namedFlags.GetInt("GE");
            ExactSize = namedFlags.GetInt("EQ");
            Token = namedFlags.GetString("TO");
            var itemType = namedFlags.GetNamedInt("TY");
            SearchItemType = itemType.IsUndefined ? ItemType.Any : (itemType.Value == 1 ? ItemType.File : ItemType.Directory);
            Tth = namedFlags.GetString("TD");
        }

        public IList<string> Include { get; }

        public IList<string> Exclude { get; }

        public IList<string> Extensions { get; }

        public int? LessThanOrEqualSize { get; set; }

        public int? GreaterThanOrEqualSize { get; set; }

        public int? ExactSize { get; set; }

        public string Token { get; set; }

        public ItemType SearchItemType { get; set; }

        public string Tth { get; set; }

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags();
            namedFlags.SetStrings("AN", Include);
            namedFlags.SetStrings("NO", Exclude);
            namedFlags.SetStrings("EX", Extensions);
            namedFlags.SetInt("LE", LessThanOrEqualSize);
            namedFlags.SetInt("GE", GreaterThanOrEqualSize);
            namedFlags.SetInt("EQ", ExactSize);
            namedFlags.SetString("TO", Token);
            namedFlags.SetInt("TY", SearchItemType == ItemType.Any ? (int?)null : (SearchItemType == ItemType.File ? 1 : 2));
            namedFlags.SetString("TD", Tth);
            return namedFlags.ToText();
        }

        public enum ItemType
        {
            Any = 0,
            File = 1,
            Directory = 2
        }
    }
}
