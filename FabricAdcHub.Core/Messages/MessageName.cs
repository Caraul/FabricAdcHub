using System.Linq;

namespace FabricAdcHub.Core.Messages
{
    public sealed class MessageName
    {
        public static readonly MessageName Status = new MessageName("STA");
        public static readonly MessageName Supports = new MessageName("SUP");
        public static readonly MessageName Sid = new MessageName("SID");
        public static readonly MessageName Information = new MessageName("INF");
        public static readonly MessageName Message = new MessageName("MSG");
        public static readonly MessageName Search = new MessageName("SCH");
        public static readonly MessageName Result = new MessageName("RES");
        public static readonly MessageName ConnectToMe = new MessageName("CTM");
        public static readonly MessageName ReversedConnectToMe = new MessageName("RCM");
        public static readonly MessageName GetPassword = new MessageName("GPA");
        public static readonly MessageName Password = new MessageName("PAS");
        public static readonly MessageName Quit = new MessageName("QUI");
        public static readonly MessageName Get = new MessageName("GET");
        public static readonly MessageName GetFileInformation = new MessageName("GFI");
        public static readonly MessageName Send = new MessageName("SND");

        public string MessageNameText { get; set; }

        public static MessageName FromText(string text)
        {
            var validTypes = new[] { Status, Supports, Sid, Information, Message, Search, Result, ConnectToMe, ReversedConnectToMe, GetPassword, Password, Quit, Get, GetFileInformation, Send };
            return validTypes.Single(validType => validType.MessageNameText == text);
        }

        public string ToText()
        {
            return MessageNameText;
        }

        private MessageName(string text)
        {
            MessageNameText = text;
        }
    }
}
