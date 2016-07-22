using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.User.Machinery.Building;

namespace FabricAdcHub.User.Machinery
{
    public class StateMachine<TState, TEvent, TEventParameter>
    {
        public StateMachine(TState initialState)
        {
            var reference = new StateReference { State = initialState };
            _stateGetter = () => Task.FromResult(reference.State);
            _stateSetter =
                value =>
                {
                    reference.State = value;
                    return Task.CompletedTask;
                };
        }

        public StateMachine(Func<Task<TState>> stateGetter, Func<TState, Task> stateSetter)
        {
            _stateGetter = stateGetter;
            _stateSetter = stateSetter;
        }

        public Task<TState> State => _stateGetter();

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

        public async Task Fire(TEvent evt, TEventParameter parameter)
        {
            var state = await State;
            var currentStateDescription = _states[state];
            foreach (var transition in currentStateDescription.Transitions)
            {
                if (transition.Trigger.Equals(evt) && await transition.Guard(evt, parameter))
                {
                    var selectedTransition = new Transition<TState, TEvent, TEventParameter>(state, transition.Destination, evt, parameter);
                    await currentStateDescription.Exit(selectedTransition);
                    await transition.Effect(evt, parameter);
                    await _stateSetter(transition.Destination);
                    var newStateDescription = _states[selectedTransition.Destination];
                    await newStateDescription.Entry(selectedTransition);
                    return;
                }
            }

            foreach (var choice in currentStateDescription.Choices)
            {
                if (choice.Trigger.Equals(evt))
                {
                    foreach (var branch in choice.Branches)
                    {
                        if (await branch.Guard(evt, parameter))
                        {
                            var selectedTransition = new Transition<TState, TEvent, TEventParameter>(state, branch.Destination, evt, parameter);
                            await currentStateDescription.Exit(selectedTransition);
                            await branch.Effect(evt, parameter);
                            await _stateSetter(branch.Destination);
                            var newStateDescription = _states[selectedTransition.Destination];
                            await newStateDescription.Entry(selectedTransition);
                            return;
                        }
                    }
                }
            }

            if (currentStateDescription.ElseTransition != null)
            {
                var selectedTransition = new Transition<TState, TEvent, TEventParameter>(state, currentStateDescription.ElseTransition.Destination, evt, parameter);
                await currentStateDescription.Exit(selectedTransition);
                await currentStateDescription.ElseTransition.Effect(evt, parameter);
                await _stateSetter(currentStateDescription.ElseTransition.Destination);
                var newStateDescription = _states[currentStateDescription.ElseTransition.Destination];
                await newStateDescription.Entry(selectedTransition);
            }
        }

        internal class StateReference
        {
            public TState State { get; set; }
        }

        private readonly Dictionary<TState, StateDescription<TState, TEvent, TEventParameter>> _states = new Dictionary<TState, StateDescription<TState, TEvent, TEventParameter>>();
        private readonly Func<Task<TState>> _stateGetter;
        private readonly Func<TState, Task> _stateSetter;
    }
}
