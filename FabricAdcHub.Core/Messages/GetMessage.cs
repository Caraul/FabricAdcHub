using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class GetMessage : Message
    {
        public GetMessage(MessageType messageType)
            : base(messageType, MessageName.Get)
        {
        }

        public GetMessage(MessageType messageType, ItemType getItemType, string identifier, int startAt, int byteCount)
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

        public bool? IsRecursive { get; set; }

        public override void FromText(IList<string> parameters)
        {
            GetItemType = parameters[0] == "file" ? ItemType.File : (parameters[0] == "list" ? ItemType.FileList : ItemType.TigerTreeHashList);
            Identifier = parameters[1];
            StartAt = int.Parse(parameters[2]);
            ByteCount = int.Parse(parameters[3]);
            var namedParameters = new NamedParameters(parameters.Skip(4));
            IsRecursive = namedParameters.GetBool("RE");
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
            var namedParameters = new NamedParameters();
            namedParameters.SetBool("RE", IsRecursive);
            return BuildString(getItemType, Identifier, StartAt.ToString(CultureInfo.InvariantCulture), ByteCount.ToString(CultureInfo.InvariantCulture), namedParameters.ToText());
        }
    }
}
