using System;
using System.Collections.Generic;
using System.Linq;
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
            var namedFlags = new NamedFlags(parameters);
            Cid = namedFlags.GetNamedString("ID");
            Pid = namedFlags.GetNamedString("PD");
            IpAddressV4 = namedFlags.GetNamedString("I4");
            IpAddressV6 = namedFlags.GetNamedString("I6");
            IpAddressV4Port = namedFlags.GetNamedInt("U4");
            IpAddressV6Port = namedFlags.GetNamedInt("U6");
            ShareSize = namedFlags.GetNamedInt("SS");
            SharedFiles = namedFlags.GetNamedInt("SF");
            AgentIdentifier = namedFlags.GetNamedString("VE");
            MaximumUploadSpeed = namedFlags.GetNamedInt("US");
            MaximumDownloadSpeed = namedFlags.GetNamedInt("DS");
            MaximumSlots = namedFlags.GetNamedInt("SL");
            AutomaticSlotAllocatorSpeedLimit = namedFlags.GetNamedInt("AS");
            MinimumSlots = namedFlags.GetNamedInt("AM");
            Email = namedFlags.GetNamedString("EM");
            Nickname = namedFlags.GetNamedString("NI");
            Description = namedFlags.GetNamedString("DE");
            HubsAsRegularUser = namedFlags.GetNamedInt("HN");
            HubsAsRegisteredUser = namedFlags.GetNamedInt("HR");
            HubsAsOperator = namedFlags.GetNamedInt("HO");
            Token = namedFlags.GetNamedString("TO");
            ClientType = namedFlags.GetNamedInt("CT").ChangeType(value => (ClientTypes)value);
            Away = namedFlags.GetNamedInt("AW");
            Features = namedFlags.GetNamedString("SU").ChangeType(value => (IList<string>)value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
            Referrer = namedFlags.GetNamedString("RF").ChangeType(value => new Uri(value));
        }

        public NamedFlag<string> Cid { get; set; }

        public NamedFlag<string> Pid { get; set; }

        public NamedFlag<string> IpAddressV4 { get; set; }

        public NamedFlag<string> IpAddressV6 { get; set; }

        public NamedFlag<int> IpAddressV4Port { get; set; }

        public NamedFlag<int> IpAddressV6Port { get; set; }

        public NamedFlag<int> ShareSize { get; set; }

        public NamedFlag<int> SharedFiles { get; set; }

        public NamedFlag<string> AgentIdentifier { get; set; }

        public NamedFlag<int> MaximumUploadSpeed { get; set; }

        public NamedFlag<int> MaximumDownloadSpeed { get; set; }

        public NamedFlag<int> MaximumSlots { get; set; }

        public NamedFlag<int> AutomaticSlotAllocatorSpeedLimit { get; set; }

        public NamedFlag<int> MinimumSlots { get; set; }

        public NamedFlag<string> Email { get; set; }

        public NamedFlag<string> Nickname { get; set; }

        public NamedFlag<string> Description { get; set; }

        public NamedFlag<int> HubsAsRegularUser { get; set; }

        public NamedFlag<int> HubsAsRegisteredUser { get; set; }

        public NamedFlag<int> HubsAsOperator { get; set; }

        public NamedFlag<string> Token { get; set; }

        public NamedFlag<ClientTypes> ClientType { get; set; }

        public NamedFlag<int> Away { get; set; }

        public NamedFlag<IList<string>> Features { get; set; }

        public NamedFlag<Uri> Referrer { get; set; }

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
            var namedFlags = new NamedFlags();
            namedFlags.SetNamedString("ID", Cid);
            namedFlags.SetNamedString("PD", Pid);
            namedFlags.SetNamedString("I4", IpAddressV4);
            namedFlags.SetNamedString("I6", IpAddressV6);
            namedFlags.SetNamedInt("U4", IpAddressV4Port);
            namedFlags.SetNamedInt("U6", IpAddressV6Port);
            namedFlags.SetNamedInt("SS", ShareSize);
            namedFlags.SetNamedInt("SF", SharedFiles);
            namedFlags.SetNamedString("VE", AgentIdentifier);
            namedFlags.SetNamedInt("US", MaximumUploadSpeed);
            namedFlags.SetNamedInt("DS", MaximumDownloadSpeed);
            namedFlags.SetNamedInt("SL", MaximumSlots);
            namedFlags.SetNamedInt("AS", AutomaticSlotAllocatorSpeedLimit);
            namedFlags.SetNamedInt("AM", MinimumSlots);
            namedFlags.SetNamedString("EM", Email);
            namedFlags.SetNamedString("NI", Nickname);
            namedFlags.SetNamedString("DE", Description);
            namedFlags.SetNamedInt("HN", HubsAsRegularUser);
            namedFlags.SetNamedInt("HR", HubsAsRegisteredUser);
            namedFlags.SetNamedInt("HO", HubsAsOperator);
            namedFlags.SetNamedString("TO", Token);
            namedFlags.SetNamedInt("CT", ClientType.ChangeType(value => (int)value));
            namedFlags.SetNamedInt("AW", Away);
            namedFlags.SetNamedString("SU", Features.ChangeType(features => string.Join(",", Features)));
            namedFlags.SetNamedString("RF", Referrer.ChangeType(referrer => Referrer.ToString()));
            return namedFlags.ToText();
        }
    }
}
