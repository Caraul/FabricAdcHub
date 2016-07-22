using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FabricAdcHub.User.Machinery.Building;

namespace FabricAdcHub.User.Machinery
{
    internal class ChoiceDescription<TState, TEvent, TEventParameter>
    {
        public ChoiceDescription(TEvent trigger)
        {
            Trigger = trigger;
        }

        public TEvent Trigger { get; }

        public HashSet<NonTriggeredTransitionDescription<TState, TEvent, TEventParameter>> Branches { get; } = new HashSet<NonTriggeredTransitionDescription<TState, TEvent, TEventParameter>>();

        public NonTriggeredTransitionDescription<TState, TEvent, TEventParameter> ElseBranch { get; private set; }

        internal class ChoiceBuilder : IChoiceBuilder<TState, TEvent, TEventParameter>
        {
            private readonly ChoiceDescription<TState, TEvent, TEventParameter> _choiceDescription;

            internal ChoiceBuilder(ChoiceDescription<TState, TEvent, TEventParameter> choiceDescription)
            {
                _choiceDescription = choiceDescription;
            }

            public IChoiceBuilder<TState, TEvent, TEventParameter> SwitchTo(TState destination, Func<TEvent, TEventParameter, Task<bool>> guard)
            {
                return SwitchTo(destination, guard, null);
            }

            public IChoiceBuilder<TState, TEvent, TEventParameter> SwitchTo(TState destination, Func<TEvent, TEventParameter, Task<bool>> guard, Func<TEvent, TEventParameter, Task> effect)
            {
                var transition = new NonTriggeredTransitionDescription<TState, TEvent, TEventParameter>(destination, guard, effect);
                _choiceDescription.Branches.Add(transition);
                return this;
            }

            public IChoiceBuilder<TState, TEvent, TEventParameter> ElseSwitchTo(TState destination)
            {
                return ElseSwitchTo(destination, null);
            }

            public IChoiceBuilder<TState, TEvent, TEventParameter> ElseSwitchTo(TState destination, Func<TEvent, TEventParameter, Task> effect)
            {
                var transition = new NonTriggeredTransitionDescription<TState, TEvent, TEventParameter>(destination, null, effect);
                _choiceDescription.ElseBranch = transition;
                return this;
            }
        }
    }
}
