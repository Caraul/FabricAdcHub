using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;

namespace FabricAdcHub.Core.Commands
{
    public sealed class GetFileInformation : Command
    {
        public GetFileInformation(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.GetFileInformation, namedParameters, originalMessage)
        {
            GetItemType = positionalParameters[0] == "file" ? ItemType.File : (positionalParameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList);
            Identifier = positionalParameters[1];
        }

        public GetFileInformation(MessageHeader header, ItemType getItemType, string identifier)
            : base(header, CommandType.GetFileInformation)
        {
            GetItemType = getItemType;
            Identifier = identifier;
        }

        public ItemType GetItemType { get; }

        public string Identifier { get; }

        public enum ItemType
        {
            File,
            FileList,
            TigerTreeHashList
        }

        protected override string GetPositionalParametersText()
        {
            var getItemType = GetItemType == ItemType.File ? "file" : (GetItemType == ItemType.FileList ? "list" : "tthl");
            return MessageSerializer.BuildText(getItemType, Identifier.Escape());
        }
    }
}
