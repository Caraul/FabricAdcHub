using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Get : Command
    {
        public Get(MessageHeader header, IList<string> parameters)
            : this(
                  header,
                  parameters[0] == "file" ? ItemType.File : (parameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList),
                  parameters[1],
                  int.Parse(parameters[2]),
                  int.Parse(parameters[3]))
        {
            var namedFlags = new NamedFlags(parameters.Skip(4));
            namedFlags.Get(IsRecursive);
        }

        public Get(MessageHeader header, ItemType getItemType, string identifier, int startAt, int byteCount)
            : base(header, CommandType.Get)
        {
            GetItemType = getItemType;
            Identifier = identifier;
            StartAt = startAt;
            ByteCount = byteCount;
        }

        public ItemType GetItemType { get; }

        public string Identifier { get; }

        public int StartAt { get; }

        public int ByteCount { get; }

        public NamedFlag<bool> IsRecursive { get; } = new NamedFlag<bool>("RE");

        public enum ItemType
        {
            File,
            FileList,
            TigerTreeHashList
        }

        protected override string GetParametersText()
        {
            var getItemType = GetItemType == ItemType.File ? "file" : (GetItemType == ItemType.FileList ? "list" : "tthl");
            var namedFlags = new NamedFlags();
            namedFlags.Set(IsRecursive);
            return BuildString(getItemType, Identifier, StartAt.ToString(CultureInfo.InvariantCulture), ByteCount.ToString(CultureInfo.InvariantCulture), namedFlags.ToText());
        }
    }
}
