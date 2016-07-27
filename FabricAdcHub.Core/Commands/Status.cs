using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Status : Command
    {
        public Status(MessageHeader header, IList<string> parameters)
            : this(
                  header,
                  (ErrorSeverity)int.Parse(parameters[0].Substring(0, 1)),
                  (ErrorCode)int.Parse(parameters[0].Substring(1, 2)),
                  parameters[1].Unescape())
        {
            new NamedFlags(parameters.Skip(2))
                .Get(OffendingCommandOrMissingFeature)
                .Get(SecondsUntilEndOfBan)
                .Get(Token)
                .Get(Protocol)
                .Get(MissingInfField)
                .Get(InvalidInfField)
                .Get(InvalidInfIpv4)
                .Get(InvalidInfIpv6);
        }

        public Status(MessageHeader header, ErrorSeverity errorSeverity, ErrorCode errorCode, string description)
            : base(header, CommandType.Status)
        {
            Severity = errorSeverity;
            Code = errorCode;
            Description = description;
        }

        public ErrorSeverity Severity { get; }

        public ErrorCode Code { get; }

        public string Description { get; }

        public NamedFlag<string> OffendingCommandOrMissingFeature { get; } = new NamedFlag<string>("FC");

        public NamedFlag<int> SecondsUntilEndOfBan { get; } = new NamedFlag<int>("TL");

        public NamedFlag<string> Token { get; } = new NamedFlag<string>("TO");

        public NamedFlag<string> Protocol { get; } = new NamedFlag<string>("PR");

        public NamedFlag<string> MissingInfField { get; } = new NamedFlag<string>("FM");

        public NamedFlag<string> InvalidInfField { get; } = new NamedFlag<string>("FB");

        public NamedFlag<string> InvalidInfIpv4 { get; } = new NamedFlag<string>("I4");

        public NamedFlag<string> InvalidInfIpv6 { get; } = new NamedFlag<string>("I6");

        public enum ErrorSeverity
        {
            Success = 0,
            Recoverable = 1,
            Fatal
        }

        public enum ErrorCode
        {
            NoError = 0,
            GenericHubError = 10,
            HubFull = 11,
            HubDisabled = 12,
            GenericLoginOrAccessError = 20,
            NickInvalid = 21,
            NickTaken = 22,
            InvalidPassword = 23,
            CidTaken = 24,
            AccessDenied = 25,
            RegisteredUsersOnly = 26,
            InvalidPidSupplied = 27,
            GenericKickOrBanOrDisconnectError = 30,
            PermanentlyBanned = 31,
            TemporarilyBanned = 32,
            ProtocolError = 40,
            UnsupportedTransferProtocol = 41,
            FailedDirectConnection = 42,
            RequiredInfFieldIsMissingOrBad = 43,
            InvalidState = 44,
            RequiredFeatureIsMissing = 45,
            InvalidIp = 46,
            NoHashSupportOverlapBetweenClientAndHub = 47,
            FileTransferError = 50,
            FileNotAvailable = 51,
            FilePartNotAvailable = 52,
            SlotsFull = 53,
            NoHashSupportOverlapBetweenClients = 54
        }

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags()
                .Set(OffendingCommandOrMissingFeature)
                .Set(SecondsUntilEndOfBan)
                .Set(Token)
                .Set(Protocol)
                .Set(MissingInfField)
                .Set(InvalidInfField)
                .Set(InvalidInfIpv4)
                .Set(InvalidInfIpv6);
            var codeText = $"{Severity:d}{Code:d}";
            return BuildString(codeText, Description.Escape(), namedFlags.ToText());
        }
    }
}
