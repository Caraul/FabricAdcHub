using System.Linq;

namespace FabricAdcHub.Core.Commands
{
    public sealed class CommandType
    {
        public static readonly CommandType Status = new CommandType("STA", 2);
        public static readonly CommandType Supports = new CommandType("SUP", 0);
        public static readonly CommandType Sid = new CommandType("SID", 0);
        public static readonly CommandType Information = new CommandType("INF", 0);
        public static readonly CommandType Message = new CommandType("MSG", 1);
        public static readonly CommandType Search = new CommandType("SCH", 0);
        public static readonly CommandType Result = new CommandType("RES", 1);
        public static readonly CommandType ConnectToMe = new CommandType("CTM", 3);
        public static readonly CommandType ReversedConnectToMe = new CommandType("RCM", 2);
        public static readonly CommandType GetPassword = new CommandType("GPA", 1);
        public static readonly CommandType Password = new CommandType("PAS", 1);
        public static readonly CommandType Quit = new CommandType("QUI", 1);
        public static readonly CommandType Get = new CommandType("GET", 4);
        public static readonly CommandType GetFileInformation = new CommandType("GFI", 2);
        public static readonly CommandType Send = new CommandType("SND", 4);

        public string Name { get; }

        public int NumberOfParameters { get; }

        public static CommandType FromText(string text)
        {
            var validTypes = new[] { Status, Supports, Sid, Information, Message, Search, Result, ConnectToMe, ReversedConnectToMe, GetPassword, Password, Quit, Get, GetFileInformation, Send };
            return validTypes.Single(validType => validType.Name == text);
        }

        public string ToText()
        {
            return Name;
        }

        private CommandType(string text, int numberOfParameters)
        {
            Name = text;
            NumberOfParameters = numberOfParameters;
        }
    }
}
