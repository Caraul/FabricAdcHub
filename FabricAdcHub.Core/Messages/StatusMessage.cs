using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;
using FabricAdcHub.Core.Utilites;

namespace FabricAdcHub.Core.Messages
{
    public sealed class StatusMessage : Message
    {
        public StatusMessage(MessageType messageType)
            : base(messageType, MessageName.Status)
        {
        }

        public StatusMessage(MessageType messageType, ErrorSeverity errorSeverity, ErrorCode errorCode, string description)
            : this(messageType)
        {
            Severity = errorSeverity;
            Code = errorCode;
            Description = description;
        }

        public ErrorSeverity Severity { get; private set; }

        public ErrorCode Code { get; private set; }

        public string Description { get; private set; }

        public string OffendingMessageOrMissingFeature { get; set; }

        public int? SecondsUntilEndOfBan { get; set; }

        public string Token { get; set; }

        public string Protocol { get; set; }

        public string MissingInfField { get; set; }

        public string InvalidInfField { get; set; }

        public string InvalidInfIpv4 { get; set; }

        public string InvalidInfIpv6 { get; set; }

        public override void FromText(IList<string> parameters)
        {
            var codeText = parameters[0];
            Severity = (ErrorSeverity)int.Parse(codeText.Substring(0, 1));
            Code = (ErrorCode)int.Parse(codeText.Substring(1, 2));
            Description = parameters[1].Unescape();

            var namedParameters = new NamedParameters(parameters.Skip(2));
            OffendingMessageOrMissingFeature = namedParameters.GetString("FC");
            SecondsUntilEndOfBan = namedParameters.GetInt("TL");
            Token = namedParameters.GetString("TO");
            Protocol = namedParameters.GetString("PR");
            MissingInfField = namedParameters.GetString("FM");
            InvalidInfField = namedParameters.GetString("FB");
            InvalidInfIpv4 = namedParameters.GetString("I4");
            InvalidInfIpv6 = namedParameters.GetString("I6");
        }

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

        protected override string GetParameters()
        {
            var namedParameters = new NamedParameters();
            namedParameters.SetString("FC", OffendingMessageOrMissingFeature);
            namedParameters.SetInt("TL", SecondsUntilEndOfBan);
            namedParameters.SetString("TO", Token);
            namedParameters.SetString("PR", Protocol);
            namedParameters.SetString("FM", MissingInfField);
            namedParameters.SetString("FB", InvalidInfField);
            namedParameters.SetString("I4", InvalidInfIpv4);
            namedParameters.SetString("I6", InvalidInfIpv6);
            var codeText = $"{Severity}{Code}";
            return BuildString(codeText, Description.Escape(), namedParameters.ToText());
        }
    }
}
