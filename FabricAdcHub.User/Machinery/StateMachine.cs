using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.User.Machinery.Building;

namespace FabricAdcHub.User.Machinery
{
    public class StateMachine<TState, TEvent, TEventParameter>
    {
        private readonly Dictionary<TState, StateDescription<TState, TEvent, TEventParameter>> _states = new Dictionary<TState, StateDescription<TState, TEvent, TEventParameter>>();
        private readonly Func<TState> _stateGetter;
        private readonly Action<TState> _stateSetter;

        public StateMachine(TState initialState)
        {
            var reference = new StateReference { State = initialState };
            _stateGetter = () => reference.State;
            _stateSetter = s => reference.State = s;
        }

        public StateMachine(Func<TState> stateGetter, Action<TState> stateSetter)
        {
            _stateGetter = stateGetter;
            _stateSetter = stateSetter;
        }

        public TState State
        {
            get
            {
                return _stateGetter();
            }

            private set
            {
                _stateSetter(value);
            }
        }

        public IStateBuilder<TState, TEvent, TEventParameter> ConfigureState(TState state)
        {
            StateDescription<TState, TEvent, TEventParameter> result;
            if (!_states.TryGetValue(state, out result))
            {
                result = new StateDescription<TState, TEvent, TEventParameter>(state);
                _states.Add(state, result);
            }

            return new StateDescription<TState, TEvent, TEventParameter>.StateBuilder(result);
        }

        public async Task ForceState(TState state)
        {
            if (State.Equals(state))
            {
                return;
            }

            var currentStateDescription = _states[State];
            var forcedTransition = new Transition<TState, TEvent, TEventParameter>(State, state, default(TEvent), default(TEventParameter));
            await currentStateDescription.Exit(forcedTransition);
            State = state;
            var newStateDescription = _states[State];
            await newStateDescription.Entry(forcedTransition);
        }

        public async Task Fire(TEvent evt, TEventParameter parameter)
        {
            var currentStateDescription = _states[State];
            foreach (var transition in currentStateDescription.Transitions)
            {
                if (transition.Trigger.Equals(evt) && await transition.Guard(evt, parameter))
                {
                    var selectedTransition = new Transition<TState, TEvent, TEventParameter>(State, transition.Destination, evt, parameter);
                    await currentStateDescription.Exit(selectedTransition);
                    await transition.Effect(evt, parameter);
                    State = transition.Destination;
                    var newStateDescription = _states[State];
                    await newStateDescription.Entry(selectedTransition);
                    return;
                }
            }
        }

        internal class StateReference
        {
            public TState State { get; set; }
        }
    }
}
