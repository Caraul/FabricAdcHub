﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;
using FabricAdcHub.User.Transitions;

namespace FabricAdcHub.User
{
    internal class AdcStateMachine
    {
        public AdcStateMachine(User user)
        {
            _states[AdcProtocolState.Unknown] = new UnknownState(user);
            _states[AdcProtocolState.Protocol] = new ProtocolState(user);
            _states[AdcProtocolState.Identify] = new IdentifyState(user);
            _states[AdcProtocolState.Normal] = new NormalState(user);

            _stateMachine = new StateMachine<AdcProtocolState, StateMachineEvent, Command>(
                () => user.StateManager.GetOrAddStateAsync(StoredState, AdcProtocolState.Unknown),
                state => user.StateManager.SetStateAsync(StoredState, state));

            _stateMachine
                .ConfigureState(_states[AdcProtocolState.Unknown])
                .SwitchTo(AdcProtocolState.Protocol, StateMachineEvent.ClientOpened)
                .ElseSwitchTo(AdcProtocolState.Unknown);

            _stateMachine
                .ConfigureState(_states[AdcProtocolState.Protocol])
                .ConfigureIfChoice(new SupportsChoice(user))
                .ConfigureIfChoice(new ProtocolStatusChoice(user))
                .ConfigureElseTransition(new ProtocolToUnknownTransition(user));

            _stateMachine
                .ConfigureState(_states[AdcProtocolState.Identify])
                .ConfigureIfChoice(new IdentifyInfChoice(user))
                .ConfigureIfChoice(new IdentifyStatusChoice(user))
                .ConfigureElseTransition(new IdentifyToUnknownTransition(user));

            _stateMachine
                .ConfigureState(_states[AdcProtocolState.Normal])
                .ConfigureTransition(new NormalToUnknownTransition(user))
                .ConfigureIfChoice(new NormalStatusChoice(user))
                .ConfigureElseTransition(new NormalProcessing(user));
        }

        public async Task Open()
        {
            await _stateMachine.Fire(StateMachineEvent.ClientOpened, default(Command));
        }

        public async Task ProcessCommand(Command command)
        {
            await _stateMachine.Fire(new StateMachineEvent(InternalEvent.AdcMessageReceived, command.Type), command);
        }

        public async Task ConnectionTimedOut()
        {
            await _stateMachine.Fire(StateMachineEvent.ConnectionTimedOut, default(Command));
        }

        public async Task CloseOnDisconnect()
        {
            await _stateMachine.Fire(StateMachineEvent.DisconnectOccured, default(Command));
        }

        private const string StoredState = "state";

        private readonly StateMachine<AdcProtocolState, StateMachineEvent, Command> _stateMachine;
        private readonly Dictionary<AdcProtocolState, StateBase<AdcProtocolState, StateMachineEvent, Command>> _states = new Dictionary<AdcProtocolState, StateBase<AdcProtocolState, StateMachineEvent, Command>>();
    }
}
