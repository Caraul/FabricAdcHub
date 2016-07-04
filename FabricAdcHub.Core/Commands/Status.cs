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
            var namedFlags = new NamedFlags(parameters.Skip(2));
            OffendingMessageOrMissingFeature = namedFlags.GetString("FC");
            SecondsUntilEndOfBan = namedFlags.GetInt("TL");
            Token = namedFlags.GetString("TO");
            Protocol = namedFlags.GetString("PR");
            MissingInfField = namedFlags.GetString("FM");
            InvalidInfField = namedFlags.GetString("FB");
            InvalidInfIpv4 = namedFlags.GetString("I4");
            InvalidInfIpv6 = namedFlags.GetString("I6");
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

        public string OffendingMessageOrMissingFeature { get; set; }

        public int? SecondsUntilEndOfBan { get; set; }

        public string Token { get; set; }

        public string Protocol { get; set; }

        public string MissingInfField { get; set; }

        public string InvalidInfField { get; set; }

        public string InvalidInfIpv4 { get; set; }

        public string InvalidInfIpv6 { get; set; }

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
            var namedFlags = new NamedFlags();
            namedFlags.SetString("FC", OffendingMessageOrMissingFeature);
            namedFlags.SetInt("TL", SecondsUntilEndOfBan);
            namedFlags.SetString("TO", Token);
            namedFlags.SetString("PR", Protocol);
            namedFlags.SetString("FM", MissingInfField);
            namedFlags.SetString("FB", InvalidInfField);
            namedFlags.SetString("I4", InvalidInfIpv4);
            namedFlags.SetString("I6", InvalidInfIpv6);
            var codeText = $"{Severity}{Code}";
            return BuildString(codeText, Description.Escape(), namedFlags.ToText());
        }
    }
}
