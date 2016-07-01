using System;
using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public sealed class InformationMessage : Message
    {
        public InformationMessage(MessageType messageType)
            : base(messageType, MessageName.Information)
        {
        }

        public NamedParameter<string> Cid { get; set; }

        public NamedParameter<string> Pid { get; set; }

        public NamedParameter<string> IpAddressV4 { get; set; }

        public NamedParameter<string> IpAddressV6 { get; set; }

        public NamedParameter<int> IpAddressV4Port { get; set; }

        public NamedParameter<int> IpAddressV6Port { get; set; }

        public NamedParameter<int> ShareSize { get; set; }

        public NamedParameter<int> SharedFiles { get; set; }

        public NamedParameter<string> AgentIdentifier { get; set; }

        public NamedParameter<int> MaximumUploadSpeed { get; set; }

        public NamedParameter<int> MaximumDownloadSpeed { get; set; }

        public NamedParameter<int> MaximumSlots { get; set; }

        public NamedParameter<int> AutomaticSlotAllocatorSpeedLimit { get; set; }

        public NamedParameter<int> MinimumSlots { get; set; }

        public NamedParameter<string> Email { get; set; }

        public NamedParameter<string> Nickname { get; set; }

        public NamedParameter<string> Description { get; set; }

        public NamedParameter<int> HubsAsRegularUser { get; set; }

        public NamedParameter<int> HubsAsRegisteredUser { get; set; }

        public NamedParameter<int> HubsAsOperator { get; set; }

        public NamedParameter<string> Token { get; set; }

        public NamedParameter<ClientTypes> ClientType { get; set; }

        public NamedParameter<int> Away { get; set; }

        public NamedParameter<IList<string>> Features { get; set; }

        public NamedParameter<Uri> Referrer { get; set; }

        public override void FromText(IList<string> parameters)
        {
            var namedParameters = new NamedParameters(parameters);
            Cid = namedParameters.GetNamedString("ID");
            Pid = namedParameters.GetNamedString("PD");
            IpAddressV4 = namedParameters.GetNamedString("I4");
            IpAddressV6 = namedParameters.GetNamedString("I6");
            IpAddressV4Port = namedParameters.GetNamedInt("U4");
            IpAddressV6Port = namedParameters.GetNamedInt("U6");
            ShareSize = namedParameters.GetNamedInt("SS");
            SharedFiles = namedParameters.GetNamedInt("SF");
            AgentIdentifier = namedParameters.GetNamedString("VE");
            MaximumUploadSpeed = namedParameters.GetNamedInt("US");
            MaximumDownloadSpeed = namedParameters.GetNamedInt("DS");
            MaximumSlots = namedParameters.GetNamedInt("SL");
            AutomaticSlotAllocatorSpeedLimit = namedParameters.GetNamedInt("AS");
            MinimumSlots = namedParameters.GetNamedInt("AM");
            Email = namedParameters.GetNamedString("EM");
            Nickname = namedParameters.GetNamedString("NI");
            Description = namedParameters.GetNamedString("DE");
            HubsAsRegularUser = namedParameters.GetNamedInt("HN");
            HubsAsRegisteredUser = namedParameters.GetNamedInt("HR");
            HubsAsOperator = namedParameters.GetNamedInt("HO");
            Token = namedParameters.GetNamedString("TO");
            ClientType = namedParameters.GetNamedInt("CT").ChangeType(value => (ClientTypes)value);
            Away = namedParameters.GetNamedInt("AW");
            Features = namedParameters.GetNamedString("SU").ChangeType(value => (IList<string>)value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
            Referrer = namedParameters.GetNamedString("RF").ChangeType(value => new Uri(value));
        }

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

        protected override string GetParameters()
        {
            var namedParameters = new NamedParameters();
            namedParameters.SetNamedString("ID", Cid);
            namedParameters.SetNamedString("PD", Pid);
            namedParameters.SetNamedString("I4", IpAddressV4);
            namedParameters.SetNamedString("I6", IpAddressV6);
            namedParameters.SetNamedInt("U4", IpAddressV4Port);
            namedParameters.SetNamedInt("U6", IpAddressV6Port);
            namedParameters.SetNamedInt("SS", ShareSize);
            namedParameters.SetNamedInt("SF", SharedFiles);
            namedParameters.SetNamedString("VE", AgentIdentifier);
            namedParameters.SetNamedInt("US", MaximumUploadSpeed);
            namedParameters.SetNamedInt("DS", MaximumDownloadSpeed);
            namedParameters.SetNamedInt("SL", MaximumSlots);
            namedParameters.SetNamedInt("AS", AutomaticSlotAllocatorSpeedLimit);
            namedParameters.SetNamedInt("AM", MinimumSlots);
            namedParameters.SetNamedString("EM", Email);
            namedParameters.SetNamedString("NI", Nickname);
            namedParameters.SetNamedString("DE", Description);
            namedParameters.SetNamedInt("HN", HubsAsRegularUser);
            namedParameters.SetNamedInt("HR", HubsAsRegisteredUser);
            namedParameters.SetNamedInt("HO", HubsAsOperator);
            namedParameters.SetNamedString("TO", Token);
            namedParameters.SetNamedInt("CT", ClientType.ChangeType(value => (int)value));
            namedParameters.SetNamedInt("AW", Away);
            namedParameters.SetNamedString("SU", Features.ChangeType(features => string.Join(",", Features)));
            namedParameters.SetNamedString("RF", Referrer.ChangeType(referrer => Referrer.ToString()));
            return namedParameters.ToText();
        }
    }
}
