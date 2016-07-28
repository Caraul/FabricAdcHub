using System.Collections.Generic;
using System.Globalization;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Send : Command
    {
        public Send(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Send, namedParameters, originalMessage)
        {
            GetItemType = positionalParameters[0] == "file" ? ItemType.File : (positionalParameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList);
            Identifier = positionalParameters[1];
            StartAt = int.Parse(positionalParameters[2]);
            ByteCount = int.Parse(positionalParameters[3]);
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

        protected override string GetPositionalParametersText()
        {
            var getItemType = GetItemType == ItemType.File ? "file" : (GetItemType == ItemType.FileList ? "list" : "tthl");
            return MessageSerializer.BuildText(getItemType, Identifier, StartAt.ToString(CultureInfo.InvariantCulture), ByteCount.ToString(CultureInfo.InvariantCulture));
        }
    }
}
