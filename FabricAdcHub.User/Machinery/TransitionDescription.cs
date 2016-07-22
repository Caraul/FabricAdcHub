using System;
using System.Threading.Tasks;

namespace FabricAdcHub.User.Machinery
{
    internal class TransitionDescription<TState, TEvent, TEventParameter> : NonTriggeredTransitionDescription<TState, TEvent, TEventParameter>
    {
        public TransitionDescription(
            TState destination,
            TEvent trigger,
            Func<TEvent, TEventParameter, Task<bool>> guard = null,
            Func<TEvent, TEventParameter, Task> effect = null)
            : base(destination, guard, effect)
        {
            Trigger = trigger;
        }

        public TEvent Trigger { get; }
    }
}
