using System.Collections.Generic;
using System.Globalization;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class SendMessage : Message
    {
        public SendMessage(MessageType messageType)
            : base(messageType, MessageName.Send)
        {
        }

        public SendMessage(MessageType messageType, ItemType getItemType, string identifier, int startAt, int byteCount)
            : this(messageType)
        {
            GetItemType = getItemType;
            Identifier = identifier;
            StartAt = startAt;
            ByteCount = byteCount;
        }

        public ItemType GetItemType { get; private set; }

        public string Identifier { get; private set; }

        public int StartAt { get; private set; }

        public int ByteCount { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            GetItemType = parameters[0] == "file" ? ItemType.File : (parameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList);
            Identifier = parameters[1];
            StartAt = int.Parse(parameters[2]);
            ByteCount = int.Parse(parameters[3]);
        }

        public enum ItemType
        {
            File,
            FileList,
            TigerTreeHashList
        }

        protected override string GetParameters()
        {
            var getItemType = GetItemType == ItemType.File ? "file" : (GetItemType == ItemType.FileList ? "list" : "tthl");
            return BuildString(getItemType, Identifier, StartAt.ToString(CultureInfo.InvariantCulture), ByteCount.ToString(CultureInfo.InvariantCulture));
        }
    }
}
