using System;
using System.Collections.Generic;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.User
{
    public class UserInformation
    {
        public UserInformation()
        {
        }

        public UserInformation(Information message)
        {
            UpdateFromCommand(message);
        }

        public string Pid { get; set; }

        public string Cid { get; set; }

        public string IpAddressV4 { get; set; }

        public string IpAddressV6 { get; set; }

        public int? IpAddressV4Port { get; set; }

        public int? IpAddressV6Port { get; set; }

        public int? ShareSize { get; set; }

        public int? SharedFiles { get; set; }

        public int? MaximumUploadSpeed { get; set; }

        public int? MaximumDownloadSpeed { get; set; }

        public int? MaximumSlots { get; set; }

        public int? AutomaticSlotAllocatorSpeedLimit { get; set; }

        public int? MinimumSlots { get; set; }

        public int? HubsAsRegularUser { get; set; }

        public int? HubsAsRegisteredUser { get; set; }

        public int? HubsAsOperator { get; set; }

        public string AgentIdentifier { get; set; }

        public string Email { get; set; }

        public string Nickname { get; set; }

        public string Description { get; set; }

        public HashSet<string> Features { get; set; }

        public string Token { get; set; }

        public Information.ClientTypes? ClientType { get; set; }

        public Information.AwayState? Away { get; set; }

        public Uri Referrer { get; set; }

        public void UpdateFromCommand(Information message)
        {
            Cid = GetValueOrDefault(message.Cid, Cid);
            Pid = GetValueOrDefault(message.Pid, Pid);
            IpAddressV4 = GetValueOrDefault(message.IpAddressV4, IpAddressV4);
            IpAddressV6 = GetValueOrDefault(message.IpAddressV6, IpAddressV6);
            IpAddressV4Port = GetValueOrDefault(message.IpAddressV4Port, IpAddressV4Port);
            IpAddressV6Port = GetValueOrDefault(message.IpAddressV6Port, IpAddressV6Port);
            ShareSize = GetValueOrDefault(message.ShareSize, ShareSize);
            SharedFiles = GetValueOrDefault(message.SharedFiles, SharedFiles);
            MaximumUploadSpeed = GetValueOrDefault(message.MaximumUploadSpeed, MaximumUploadSpeed);
            MaximumDownloadSpeed = GetValueOrDefault(message.MaximumDownloadSpeed, MaximumDownloadSpeed);
            MaximumSlots = GetValueOrDefault(message.MaximumSlots, MaximumSlots);
            AutomaticSlotAllocatorSpeedLimit = GetValueOrDefault(message.AutomaticSlotAllocatorSpeedLimit, AutomaticSlotAllocatorSpeedLimit);
            MinimumSlots = GetValueOrDefault(message.MinimumSlots, MinimumSlots);
            HubsAsRegularUser = GetValueOrDefault(message.HubsAsRegularUser, HubsAsRegularUser);
            HubsAsRegisteredUser = GetValueOrDefault(message.HubsAsRegisteredUser, HubsAsRegisteredUser);
            HubsAsOperator = GetValueOrDefault(message.HubsAsOperator, HubsAsOperator);
            AgentIdentifier = GetValueOrDefault(message.AgentIdentifier, AgentIdentifier);
            Email = GetValueOrDefault(message.Email, Email);
            Nickname = GetValueOrDefault(message.Nickname, Nickname);
            Description = GetValueOrDefault(message.Description, Description);
            Features = GetValueOrDefault(message.Features, Features);
            Token = GetValueOrDefault(message.Token, Token);
            ClientType = GetValueOrDefault(message.ClientType, ClientType);
            Away = GetValueOrDefault(message.Away, Away);
            Referrer = GetValueOrDefault(message.Referrer, Referrer);
        }

        public void UpdateFromCommand(Supports message)
        {
            Features.ExceptWith(message.RemoveFeatures);
            Features.UnionWith(message.AddFeatures);
        }

        public Information ToCommand(MessageHeader messageType)
        {
            var infromation = new Information(messageType);
            infromation.Cid.FromNullable(Cid);
            infromation.Pid.State = NamedFlagState.Undefined;
            infromation.IpAddressV4.FromNullable(IpAddressV4);
            infromation.IpAddressV6.FromNullable(IpAddressV6);
            infromation.IpAddressV4Port.FromNullable(IpAddressV4Port);
            infromation.IpAddressV6Port.FromNullable(IpAddressV6Port);
            infromation.ShareSize.FromNullable(ShareSize);
            infromation.SharedFiles.FromNullable(SharedFiles);
            infromation.AgentIdentifier.FromNullable(AgentIdentifier);
            infromation.MaximumUploadSpeed.FromNullable(MaximumUploadSpeed);
            infromation.MaximumDownloadSpeed.FromNullable(MaximumDownloadSpeed);
            infromation.MaximumSlots.FromNullable(MaximumSlots);
            infromation.AutomaticSlotAllocatorSpeedLimit.FromNullable(AutomaticSlotAllocatorSpeedLimit);
            infromation.MinimumSlots.FromNullable(MinimumSlots);
            infromation.Email.FromNullable(Email);
            infromation.Nickname.FromNullable(Nickname);
            infromation.Description.FromNullable(Description);
            infromation.HubsAsRegularUser.FromNullable(HubsAsRegularUser);
            infromation.HubsAsRegisteredUser.FromNullable(HubsAsRegisteredUser);
            infromation.HubsAsOperator.FromNullable(HubsAsOperator);
            infromation.Token.FromNullable(Token);
            infromation.ClientType.FromNullable(ClientType);
            infromation.Away.FromNullable(Away);
            infromation.Features.FromNullable(Features);
            infromation.Referrer.FromNullable(Referrer);
            return infromation;
        }

        private static TValue? GetValueOrDefault<TValue>(NamedFlag<TValue> value, TValue? defaultValue)
            where TValue : struct
        {
            if (value.IsUndefined)
            {
                return defaultValue;
            }

            return value.IsDropped ? (TValue?)null : value.Value;
        }

        private static TValue GetValueOrDefault<TValue>(NamedFlag<TValue> value, TValue defaultValue)
            where TValue : class
        {
            if (value.IsUndefined)
            {
                return defaultValue;
            }

            return value.IsDropped ? null : value.Value;
        }
    }
}
