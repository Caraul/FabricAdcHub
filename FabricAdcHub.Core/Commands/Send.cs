using System.Collections.Generic;
using System.Globalization;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Send : Command
    {
        public Send(MessageHeader header, IList<string> parameters)
            : this(
                  header,
                  parameters[0] == "file" ? ItemType.File : (parameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList),
                  parameters[1],
                  int.Parse(parameters[2]),
                  int.Parse(parameters[3]))
        {
        }

        public Send(MessageHeader header, ItemType getItemType, string identifier, int startAt, int byteCount)
            : base(header, CommandType.Send)
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

        public enum ItemType
        {
            File,
            FileList,
            TigerTreeHashList
        }

        protected override string GetParametersText()
        {
            var getItemType = GetItemType == ItemType.File ? "file" : (GetItemType == ItemType.FileList ? "list" : "tthl");
            return BuildString(getItemType, Identifier, StartAt.ToString(CultureInfo.InvariantCulture), ByteCount.ToString(CultureInfo.InvariantCulture));
        }
    }
}
