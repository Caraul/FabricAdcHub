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
            foreach (var transitionDescription in currentStateDescription.Transitions)
            {
                if (transitionDescription.Trigger.Equals(evt) && await transitionDescription.Guard(evt, parameter))
                {
                    await TransitToState(currentStateDescription, transitionDescription, evt, parameter);
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
                            await TransitToState(currentStateDescription, branch, evt, parameter);
                            return;
                        }
                    }

                    await TransitToState(currentStateDescription, choice.ElseBranch, evt, parameter);
                    return;
                }
            }

            if (currentStateDescription.ElseTransition != null)
            {
                await TransitToState(currentStateDescription, currentStateDescription.ElseTransition, evt, parameter);
            }
        }

        internal class StateReference
        {
            public TState State { get; set; }
        }

        private async Task TransitToState(
            StateDescription<TState, TEvent, TEventParameter> sourceDescription,
            NonTriggeredTransitionDescription<TState, TEvent, TEventParameter> transitionDescription,
            TEvent evt,
            TEventParameter parameter)
        {
            var transition = new Transition<TState, TEvent, TEventParameter>(sourceDescription.State, transitionDescription.Destination, evt, parameter);
            if (transition.Source.Equals(transition.Destination))
            {
                await transitionDescription.Effect(evt, parameter);
            }
            else
            {
                await sourceDescription.Exit(transition);
                await transitionDescription.Effect(evt, parameter);
                await _stateSetter(transition.Destination);
                var newStateDescription = _states[transition.Destination];
                await newStateDescription.Entry(transition);
            }
        }

        private readonly Dictionary<TState, StateDescription<TState, TEvent, TEventParameter>> _states = new Dictionary<TState, StateDescription<TState, TEvent, TEventParameter>>();
        private readonly Func<Task<TState>> _stateGetter;
        private readonly Func<TState, Task> _stateSetter;
    }
}
