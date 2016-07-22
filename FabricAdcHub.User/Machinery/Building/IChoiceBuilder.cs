using System;
using System.Threading.Tasks;

namespace FabricAdcHub.User.Machinery.Building
{
    public interface IChoiceBuilder<in TState, out TEvent, out TEventParameter>
    {
        IChoiceBuilder<TState, TEvent, TEventParameter> SwitchTo(TState destination, Func<TEvent, TEventParameter, Task<bool>> guard, Func<TEvent, TEventParameter, Task> effect);

        IChoiceBuilder<TState, TEvent, TEventParameter> ElseSwitchTo(TState destination, Func<TEvent, TEventParameter, Task> effect);
    }
}
