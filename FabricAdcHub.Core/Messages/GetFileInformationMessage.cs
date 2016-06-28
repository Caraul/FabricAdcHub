using System.Collections.Generic;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class GetFileInformationMessage : Message
    {
        public GetFileInformationMessage(MessageType messageType)
            : base(messageType, MessageName.GetFileInformation)
        {
        }

        public GetFileInformationMessage(MessageType messageType, ItemType getItemType, string identifier)
            : this(messageType)
        {
            GetItemType = getItemType;
            Identifier = identifier;
        }

        public ItemType GetItemType { get; private set; }

        public string Identifier { get; private set; }

        public override void FromText(IList<string> parameters)
        {
            GetItemType = parameters[0] == "file" ? ItemType.File : (parameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList);
            Identifier = parameters[1];
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
            return BuildString(getItemType, Identifier);
        }
    }
}
