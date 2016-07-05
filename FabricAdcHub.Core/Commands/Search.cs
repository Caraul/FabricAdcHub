using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Search : Command
    {
        public Search(MessageHeader header)
            : base(header, CommandType.Search)
        {
        }

        public Search(MessageHeader header, IList<string> parameters)
            : this(header)
        {
            new NamedFlags(parameters)
                .Get(Include)
                .Get(Exclude)
                .Get(Extensions)
                .Get(LessThanOrEqualSize)
                .Get(GreaterThanOrEqualSize)
                .Get(ExactSize)
                .Get(Token)
                .Get(SearchItemType, () => ItemType.Any, () => ItemType.Any, value => value == "1" ? ItemType.File : ItemType.Directory)
                .Get(Tth);
        }

        public NamedFlag<HashSet<string>> Include { get; } = new NamedFlag<HashSet<string>>("AN");

        public NamedFlag<HashSet<string>> Exclude { get; } = new NamedFlag<HashSet<string>>("NO");

        public NamedFlag<HashSet<string>> Extensions { get; } = new NamedFlag<HashSet<string>>("EX");

        public NamedFlag<int> LessThanOrEqualSize { get; } = new NamedFlag<int>("LE");

        public NamedFlag<int> GreaterThanOrEqualSize { get; } = new NamedFlag<int>("GE");

        public NamedFlag<int> ExactSize { get; } = new NamedFlag<int>("EQ");

        public NamedFlag<string> Token { get; } = new NamedFlag<string>("TO");

        public NamedFlag<ItemType> SearchItemType { get; } = new NamedFlag<ItemType>("TY", ItemType.Any);

        public NamedFlag<string> Tth { get; } = new NamedFlag<string>("TD");

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags()
                .Set(Include)
                .Set(Exclude)
                .Set(Extensions)
                .Set(LessThanOrEqualSize)
                .Set(GreaterThanOrEqualSize)
                .Set(ExactSize)
                .Set(Token)
                .Set(
                    SearchItemType.State == NamedFlagState.Undefined
                    ? NamedFlag<ItemType>.Undefined(SearchItemType.Name)
                    : SearchItemType,
                    itemType => itemType == ItemType.File ? "1" : "2")
                .Set(Tth);
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
