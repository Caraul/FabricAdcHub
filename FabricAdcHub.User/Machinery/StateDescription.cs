using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.User.Machinery.Building;

namespace FabricAdcHub.User.Machinery
{
    internal class StateDescription<TState, TEvent, TEventParameter>
    {
        public StateDescription(TState state)
        {
            State = state;
        }

        public TState State { get; }

        public Func<Transition<TState, TEvent, TEventParameter>, Task> Entry { get; private set; } = EmptyMethod;

        public Func<Transition<TState, TEvent, TEventParameter>, Task> Exit { get; private set; } = EmptyMethod;

        public HashSet<TransitionDescription<TState, TEvent, TEventParameter>> Transitions { get; } = new HashSet<TransitionDescription<TState, TEvent, TEventParameter>>();

        public HashSet<ChoiceDescription<TState, TEvent, TEventParameter>> Choices { get; } = new HashSet<ChoiceDescription<TState, TEvent, TEventParameter>>();

        private static readonly Func<Transition<TState, TEvent, TEventParameter>, Task> EmptyMethod = _ => Task.CompletedTask;

        internal class StateBuilder : IStateBuilder<TState, TEvent, TEventParameter>
        {
            private readonly StateDescription<TState, TEvent, TEventParameter> _stateDescription;

            internal StateBuilder(StateDescription<TState, TEvent, TEventParameter> stateDescription)
            {
                _stateDescription = stateDescription;
            }

            public IStateBuilder<TState, TEvent, TEventParameter> OnEntry(Func<Transition<TState, TEvent, TEventParameter>, Task> entry)
            {
                _stateDescription.Entry = entry;
                return this;
            }

            public IStateBuilder<TState, TEvent, TEventParameter> OnExit(Func<Transition<TState, TEvent, TEventParameter>, Task> exit)
            {
                _stateDescription.Exit = exit;
                return this;
            }

            public IStateBuilder<TState, TEvent, TEventParameter> SwitchTo(TState destination, TEvent trigger, Func<TEvent, TEventParameter, Task<bool>> guard, Func<TEvent, TEventParameter, Task> effect)
            {
                var transition = new TransitionDescription<TState, TEvent, TEventParameter>(destination, trigger, guard, effect);
                _stateDescription.Transitions.Add(transition);
                return this;
            }

            public IChoiceBuilder<TState, TEvent, TEventParameter> ChoiceSwitchTo(TEvent trigger)
            {
                var choice = new ChoiceDescription<TState, TEvent, TEventParameter>(trigger);
                _stateDescription.Choices.Add(choice);
                return new ChoiceDescription<TState, TEvent, TEventParameter>.ChoiceBuilder(choice);
            }
        }
    }
}
