using System.Collections.Generic;
using FabricAdcHub.Core.Commands.NamedParameters;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Information : Command
    {
        public Information(MessageHeader header, IList<string> namedParameters, string originalMessage)
            : base(header, CommandType.Information, namedParameters, originalMessage)
        {
        }

        public Information(MessageHeader header)
            : base(header, CommandType.Information)
        {
        }

        public NamedString Cid => GetString("ID");

        public NamedString Pid => GetString("PD");

        public NamedString IpAddressV4 => GetString("I4");

        public NamedString IpAddressV6 => GetString("I6");

        public NamedInt IpAddressV4Port => GetInt("U4");

        public NamedInt IpAddressV6Port => GetInt("U6");

        public NamedInt ShareSize => GetInt("SS");

        public NamedInt SharedFiles => GetInt("SF");

        public NamedString AgentIdentifier => GetString("VE");

        public NamedInt MaximumUploadSpeed => GetInt("US");

        public NamedInt MaximumDownloadSpeed => GetInt("DS");

        public NamedInt MaximumSlots => GetInt("SL");

        public NamedInt AutomaticSlotAllocatorSpeedLimit => GetInt("AS");

        public NamedInt MinimumSlots => GetInt("AM");

        public NamedString Email => GetString("EM");

        public NamedString Nickname => GetString("NI");

        public NamedString Description => GetString("DE");

        public NamedInt HubsAsRegularUser => GetInt("HN");

        public NamedInt HubsAsRegisteredUser => GetInt("HR");

        public NamedInt HubsAsOperator => GetInt("HO");

        public NamedString Token => GetString("TO");

        public NamedClientTypes ClientType => new NamedClientTypes("CT", NamedFlags);

        public NamedAwayState Away => new NamedAwayState("AW", NamedFlags);

        public NamedFeatures Features => new NamedFeatures("SU", NamedFlags);

        public NamedUri Referrer => GetUri("RF");

        public enum ClientTypes
        {
            Bot = 1,
            RegisteredUser = 2,
            Operator = 4,
            SuperUser = 8,
            HubOwner = 16,
            Hub = 32
        }

        public enum AwayState
        {
            Away = 1,
            ExtendedAway = 2,
        }
    }
}
