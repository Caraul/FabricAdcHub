using System;
using System.Net;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Messages;
using FabricAdcHub.Core.MessageTypes;
using FabricAdcHub.User.Interfaces;
using FabricAdcHub.User.States;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.User
{
    [StatePersistence(StatePersistence.Volatile)]
    internal class User : Actor, IUser
    {
        public async Task<bool> TryStart(IPAddress clientIPv4, IPAddress clientIPv6)
        {
            if (!await StateManager.TryAddStateAsync("state", State.Protocol))
            {
                return false;
            }

            ClientIPv4 = clientIPv4;
            ClientIPv6 = clientIPv6;
            return true;
        }

        public IPAddress ClientIPv4 { get; private set; }

        public IPAddress ClientIPv6 { get; private set; }

        public UserInformation Information { get; private set; } = new UserInformation();

        public async Task ProcessMessage(string messageText)
        {
            var message = MessageSerializer.FromText(messageText);
            var informationMessage = message as InformationMessage;
            if (informationMessage != null)
            {
                EnrichInformationMessage(informationMessage);
            }

            var newState = await _state.ProcessMessage(message);
            if (newState != _state.State)
            {
                await StateManager.SetStateAsync("state", newState);
                SwitchToState(newState);
            }
        }

        public Task SendMessage(Message message)
        {
            return SendSerializedMessage(message.ToText());
        }

        public Task SendSerializedMessage(string messageText)
        {
            var events = GetEvent<IUserEvents>();
            events.MessageAvailable(messageText);
            return Task.CompletedTask;
        }

        public async Task<string> UpdateInformation(InformationMessage message)
        {
            Information.UpdateFromMessage(message);
            await StateManager.SetStateAsync("information", Information);
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"));
            var storedMessage = Information.ToMessage(new BroadcastMessageType { Sid = Id.GetStringId() });
            await catalog.UpdateSidInformation(Id.GetStringId(), storedMessage);
            return storedMessage.ToText();
        }

        public async Task UpdateInformation(SupportsMessage message)
        {
            Information.UpdateFromMessage(message);
            await StateManager.SetStateAsync("information", Information);
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"));
            var storedMessage = Information.ToMessage(new BroadcastMessageType { Sid = Id.GetStringId() });
            await catalog.UpdateSidInformation(Id.GetStringId(), storedMessage);
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var maybeState = await StateManager.TryGetStateAsync<State>("state");
            if (maybeState.HasValue)
            {
                var maybeInformation = await StateManager.TryGetStateAsync<UserInformation>("information");
                if (maybeInformation.HasValue)
                {
                    Information = maybeInformation.Value;
                }

                SwitchToState(maybeState.Value);
            }
        }

        private void SwitchToState(State state)
        {
            switch (state)
            {
                case State.Protocol:
                    _state = new ProtocolState(this);
                    break;
                case State.Identify:
                    _state = new IdentifyState(this);
                    break;
                case State.Verify:
                    _state = new VerifyState(this);
                    break;
                case State.Normal:
                    _state = new NormalState(this);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void EnrichInformationMessage(InformationMessage message)
        {
            if (message.IpAddressV4.IsDefined && message.IpAddressV4.Value == "0.0.0.0")
            {
                message.IpAddressV4 = new NamedParameter<string>(ClientIPv4.ToString());
            }

            if (message.IpAddressV6.IsDefined && message.IpAddressV6.Value == "::")
            {
                message.IpAddressV6 = new NamedParameter<string>(ClientIPv6.ToString());
            }
        }

        private StateBase _state;
    }
}
