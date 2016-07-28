using System.Collections.Generic;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Status : Command
    {
        public Status(MessageHeader header, IList<string> positionalParameters, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Status, namedParameters, originalMessage)
        {
            Severity = (ErrorSeverity)int.Parse(positionalParameters[0].Substring(0, 1));
            Code = (ErrorCode)int.Parse(positionalParameters[0].Substring(1, 2));
            Description = positionalParameters[1].Unescape();
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

        public NamedString OffendingCommandOrMissingFeature => GetString("FC");

        public NamedInt SecondsUntilEndOfBan => GetInt("TL");

        public NamedString Token => GetString("TO");

        public NamedString Protocol => GetString("PR");

        public NamedString MissingInfField => GetString("FM");

        public NamedString InvalidInfField => GetString("FB");

        public NamedString InvalidInfIpv4 => GetString("I4");

        public NamedString InvalidInfIpv6 => GetString("I6");

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

        protected override string GetPositionalParametersText()
        {
            var codeText = $"{Severity:d}{Code:d}";
            return MessageSerializer.BuildText(codeText, Description.Escape());
        }
    }
}
