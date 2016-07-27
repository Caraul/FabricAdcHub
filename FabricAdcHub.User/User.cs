using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Sender.Interfaces;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.User
{
    [StatePersistence(StatePersistence.Volatile)]
    internal class User : Actor, IUser
    {
        public string Sid => Id.GetStringId();

        public IPAddress ClientIPv4 { get; private set; }

        public IPAddress ClientIPv6 { get; private set; }

        public UserInformation Information { get; private set; } = new UserInformation();

        public async Task Open(IPAddress clientIPv4, IPAddress clientIPv6)
        {
            await StateManager.SetStateAsync(StoredIPv4, clientIPv4.ToString());
            ClientIPv4 = clientIPv4;
            await StateManager.SetStateAsync(StoredIPv6, clientIPv6.ToString());
            ClientIPv6 = clientIPv6;

            await _stateMachine.Open();

            ActorEventSource.Current.Opened(Sid);
        }

        public Task Close()
        {
            var sender = ActorProxy.Create<ISender>(new ActorId(Sid));
            sender.Close();

            ActorEventSource.Current.Closed(Sid);

            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"));
            return catalog.ReleaseSid(Sid);
        }

        public Task<string> GetInformationMessage()
        {
            return Task.FromResult(Information.ToCommand(new BroadcastMessageHeader(Sid)).ToMessage());
        }

        public async Task ProcessMessage(string message)
        {
            Command command;
            if (!MessageSerializer.TryCreateFromText(message, out command))
            {
                ActorEventSource.Current.CommandDeserializationFailed(message);
                return;
            }

            var information = command as Information;
            if (information != null)
            {
                EnrichInformationMessage(information);
            }

            ActorEventSource.Current.CommandReceived(Sid, message);
            await _stateMachine.ProcessCommand(command);
        }

        public async Task UpdateInformation(Information message)
        {
            Information.UpdateFromCommand(message);
            await StateManager.SetStateAsync(StoredInfomation, Information);
            await UpdateCatalog();
        }

        public async Task UpdateInformation(Supports message)
        {
            Information.UpdateFromCommand(message);
            await StateManager.SetStateAsync(StoredInfomation, Information);
            await UpdateCatalog();
        }

        public async Task CloseOnDisconnect()
        {
            await _stateMachine.DisconnectOnNetworkError();
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            ClientIPv4 = IPAddress.Parse(await StateManager.GetOrAddStateAsync(StoredIPv4, AnyIPv4));
            ClientIPv6 = IPAddress.Parse(await StateManager.GetOrAddStateAsync(StoredIPv6, AnyIPv6));

            _stateMachine = new AdcStateMachine(this);

            var maybeInformation = await StateManager.TryGetStateAsync<UserInformation>(StoredInfomation);
            if (maybeInformation.HasValue)
            {
                Information = maybeInformation.Value;
            }
        }

        private void EnrichInformationMessage(Information message)
        {
            if (message.IpAddressV4.IsDefined && message.IpAddressV4.Value == AnyIPv4)
            {
                message.IpAddressV4.Value = ClientIPv4.ToString();
            }

            if (message.IpAddressV6.IsDefined && message.IpAddressV6.Value == AnyIPv6)
            {
                message.IpAddressV6.Value = ClientIPv6.ToString();
            }
        }

        private async Task UpdateCatalog()
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"));
            await catalog.UpdateSidInformation(Sid, Information.Features);
        }

        private const string StoredIPv4 = "ipv4";
        private const string StoredIPv6 = "ipv6";
        private const string StoredInfomation = "information";

        private static readonly string AnyIPv4 = IPAddress.Any.ToString();
        private static readonly string AnyIPv6 = IPAddress.IPv6Any.ToString();

        private AdcStateMachine _stateMachine;
    }
}
