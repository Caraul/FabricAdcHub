using System;
using System.Threading.Tasks;

namespace FabricAdcHub.User.Machinery.Building
{
    public interface IStateBuilder<TState, TEvent, TEventParameter>
    {
        IStateBuilder<TState, TEvent, TEventParameter> OnEntry(Func<Transition<TState, TEvent, TEventParameter>, Task> entry);

        IStateBuilder<TState, TEvent, TEventParameter> OnExit(Func<Transition<TState, TEvent, TEventParameter>, Task> exit);

        IStateBuilder<TState, TEvent, TEventParameter> SwitchTo(
            TState destination,
            TEvent trigger);

        IStateBuilder<TState, TEvent, TEventParameter> SwitchTo(
            TState destination,
            TEvent trigger,
            Func<TEvent, TEventParameter, Task<bool>> guard,
            Func<TEvent, TEventParameter, Task> effect);

        IChoiceBuilder<TState, TEvent, TEventParameter> ChoiceSwitchTo(TEvent trigger);

        IStateBuilder<TState, TEvent, TEventParameter> ElseSwitchTo(TState destination);

        IStateBuilder<TState, TEvent, TEventParameter> ElseSwitchTo(TState destination, Func<TEvent, TEventParameter, Task> effect);
    }
}
