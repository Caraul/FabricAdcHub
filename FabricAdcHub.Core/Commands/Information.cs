using System;
using System.Collections.Generic;
using System.Globalization;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core.Commands
{
    public sealed class Information : Command
    {
        public Information(MessageHeader header)
            : base(header, CommandType.Information)
        {
        }

        public Information(MessageHeader header, IList<string> parameters)
            : this(header)
        {
            new NamedFlags(parameters)
                .Get(Cid)
                .Get(Pid)
                .Get(IpAddressV4)
                .Get(IpAddressV6)
                .Get(IpAddressV4Port)
                .Get(IpAddressV6Port)
                .Get(ShareSize)
                .Get(SharedFiles)
                .Get(AgentIdentifier)
                .Get(MaximumUploadSpeed)
                .Get(MaximumDownloadSpeed)
                .Get(MaximumSlots)
                .Get(AutomaticSlotAllocatorSpeedLimit)
                .Get(MinimumSlots)
                .Get(Email)
                .Get(Nickname)
                .Get(Description)
                .Get(HubsAsRegularUser)
                .Get(HubsAsRegisteredUser)
                .Get(HubsAsOperator)
                .Get(Token)
                .Get(ClientType, value => (ClientTypes)int.Parse(value))
                .Get(Away, value => (AwayState)int.Parse(value))
                .Get(Features, value => new HashSet<string>(value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)))
                .Get(Referrer);
        }

        public NamedFlag<string> Cid { get; } = new NamedFlag<string>("ID");

        public NamedFlag<string> Pid { get; } = new NamedFlag<string>("PD");

        public NamedFlag<string> IpAddressV4 { get; } = new NamedFlag<string>("I4");

        public NamedFlag<string> IpAddressV6 { get; } = new NamedFlag<string>("I6");

        public NamedFlag<int> IpAddressV4Port { get; } = new NamedFlag<int>("U4");

        public NamedFlag<int> IpAddressV6Port { get; } = new NamedFlag<int>("U6");

        public NamedFlag<int> ShareSize { get; } = new NamedFlag<int>("SS");

        public NamedFlag<int> SharedFiles { get; } = new NamedFlag<int>("SF");

        public NamedFlag<string> AgentIdentifier { get; } = new NamedFlag<string>("VE");

        public NamedFlag<int> MaximumUploadSpeed { get; } = new NamedFlag<int>("US");

        public NamedFlag<int> MaximumDownloadSpeed { get; } = new NamedFlag<int>("DS");

        public NamedFlag<int> MaximumSlots { get; } = new NamedFlag<int>("SL");

        public NamedFlag<int> AutomaticSlotAllocatorSpeedLimit { get; } = new NamedFlag<int>("AS");

        public NamedFlag<int> MinimumSlots { get; } = new NamedFlag<int>("AM");

        public NamedFlag<string> Email { get; } = new NamedFlag<string>("EM");

        public NamedFlag<string> Nickname { get; } = new NamedFlag<string>("NI");

        public NamedFlag<string> Description { get; } = new NamedFlag<string>("DE");

        public NamedFlag<int> HubsAsRegularUser { get; } = new NamedFlag<int>("HN");

        public NamedFlag<int> HubsAsRegisteredUser { get; } = new NamedFlag<int>("HR");

        public NamedFlag<int> HubsAsOperator { get; } = new NamedFlag<int>("HO");

        public NamedFlag<string> Token { get; } = new NamedFlag<string>("TO");

        public NamedFlag<ClientTypes> ClientType { get; } = new NamedFlag<ClientTypes>("CT");

        public NamedFlag<AwayState> Away { get; } = new NamedFlag<AwayState>("AW");

        public NamedFlag<HashSet<string>> Features { get; } = new NamedFlag<HashSet<string>>("SU");

        public NamedFlag<Uri> Referrer { get; } = new NamedFlag<Uri>("RF");

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

        protected override string GetParametersText()
        {
            var namedFlags = new NamedFlags()
                .Set(Cid)
                .Set(Pid)
                .Set(IpAddressV4)
                .Set(IpAddressV6)
                .Set(IpAddressV4Port)
                .Set(IpAddressV6Port)
                .Set(ShareSize)
                .Set(SharedFiles)
                .Set(AgentIdentifier)
                .Set(MaximumUploadSpeed)
                .Set(MaximumDownloadSpeed)
                .Set(MaximumSlots)
                .Set(AutomaticSlotAllocatorSpeedLimit)
                .Set(MinimumSlots)
                .Set(Email)
                .Set(Nickname)
                .Set(Description)
                .Set(HubsAsRegularUser)
                .Set(HubsAsRegisteredUser)
                .Set(HubsAsOperator)
                .Set(Token)
                .Set(ClientType, value => ((int)value).ToString(CultureInfo.InvariantCulture))
                .Set(Away, value => value == AwayState.Away ? "1" : "2")
                .Set(Features, features => string.Join(",", features))
                .Set(Referrer);
            return namedFlags.ToText();
        }
    }
}
