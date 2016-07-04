using System.Linq;

namespace FabricAdcHub.Core.Commands
{
    public sealed class CommandType
    {
        public static readonly CommandType Status = new CommandType("STA");
        public static readonly CommandType Supports = new CommandType("SUP");
        public static readonly CommandType Sid = new CommandType("SID");
        public static readonly CommandType Information = new CommandType("INF");
        public static readonly CommandType Message = new CommandType("MSG");
        public static readonly CommandType Search = new CommandType("SCH");
        public static readonly CommandType Result = new CommandType("RES");
        public static readonly CommandType ConnectToMe = new CommandType("CTM");
        public static readonly CommandType ReversedConnectToMe = new CommandType("RCM");
        public static readonly CommandType GetPassword = new CommandType("GPA");
        public static readonly CommandType Password = new CommandType("PAS");
        public static readonly CommandType Quit = new CommandType("QUI");
        public static readonly CommandType Get = new CommandType("GET");
        public static readonly CommandType GetFileInformation = new CommandType("GFI");
        public static readonly CommandType Send = new CommandType("SND");

        public string Name { get; }

        public static CommandType FromText(string text)
        {
            var validTypes = new[] { Status, Supports, Sid, Information, Message, Search, Result, ConnectToMe, ReversedConnectToMe, GetPassword, Password, Quit, Get, GetFileInformation, Send };
            return validTypes.Single(validType => validType.Name == text);
        }

        public string ToText()
        {
            return Name;
        }

        private CommandType(string text)
        {
            Name = text;
        }
    }
}
