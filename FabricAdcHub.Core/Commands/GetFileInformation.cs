using System.Collections.Generic;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class GetFileInformation : Command
    {
        public GetFileInformation(MessageHeader header, IList<string> parameters)
            : this(
                  header,
                  parameters[0] == "file" ? ItemType.File : (parameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList),
                  parameters[1])
        {
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

        protected override string GetParametersText()
        {
            var getItemType = GetItemType == ItemType.File ? "file" : (GetItemType == ItemType.FileList ? "list" : "tthl");
            return BuildString(getItemType, Identifier);
        }
    }
}
