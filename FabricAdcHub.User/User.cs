using System;
using System.Net;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.User.Interfaces;
using FabricAdcHub.User.States;
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

            ActorEventSource.Current.Opened(Sid);
        }

        public Task Close()
        {
            var events = GetEvent<IUserEvents>();
            events.Closed();

            ActorEventSource.Current.Closed(Sid);

            return Task.CompletedTask;
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

            var state = await _stateMachine.CurrentState.ProcessCommand(command);
            await SwitchToState(state);
        }

        public Task SendCommand(Command command)
        {
            if (!_stateMachine.CurrentState.IsSendCommandAllowed(command))
            {
                return Task.CompletedTask;
            }

            return SendMessage(command.ToMessage());
        }

        public Task SendMessage(string message)
        {
            var events = GetEvent<IUserEvents>();
            events.MessageAvailable(message);
            return Task.CompletedTask;
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

        public async Task Disconnect(DisconnectReason reason)
        {
            switch (reason)
            {
                case DisconnectReason.NetworkError:
                    await _stateMachine.SwitchToState(State.DisconnectedOnNetworkError);
                    break;
                case DisconnectReason.ProtocolError:
                    await _stateMachine.SwitchToState(State.DisconnectedOnProtocolError);
                    break;
                case DisconnectReason.HubIsDisabled:
                    await _stateMachine.SwitchToState(State.DisconnectedOnShutdown);
                    break;
                default:
                    throw new Exception();
            }
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            ClientIPv4 = IPAddress.Parse(await StateManager.GetOrAddStateAsync(StoredIPv4, AnyIPv4));
            ClientIPv6 = IPAddress.Parse(await StateManager.GetOrAddStateAsync(StoredIPv6, AnyIPv6));

            var state = await StateManager.GetOrAddStateAsync(StoredState, State.Protocol);
            ActorEventSource.Current.StateInitialized(Sid, state.ToString());
            _stateMachine = new StateMachine(this);
            await _stateMachine.Start(state);

            var maybeInformation = await StateManager.TryGetStateAsync<UserInformation>(StoredInfomation);
            if (maybeInformation.HasValue)
            {
                Information = maybeInformation.Value;
            }
        }

        private async Task SwitchToState(State state)
        {
            ActorEventSource.Current.StateChanged(Sid, _stateMachine.CurrentState.State.ToString(), state.ToString());
            await StateManager.SetStateAsync(StoredState, state);
            await _stateMachine.SwitchToState(state);
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
        private const string StoredState = "state";
        private const string StoredInfomation = "information";

        private static readonly string AnyIPv4 = IPAddress.Any.ToString();
        private static readonly string AnyIPv6 = IPAddress.IPv6Any.ToString();

        private StateMachine _stateMachine;
    }
}
