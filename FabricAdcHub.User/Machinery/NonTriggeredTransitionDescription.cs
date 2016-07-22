using System;
using System.Threading.Tasks;

namespace FabricAdcHub.User.Machinery
{
    internal class NonTriggeredTransitionDescription<TState, TEvent, TEventParameter>
    {
        public NonTriggeredTransitionDescription(
            TState destination,
            Func<TEvent, TEventParameter, Task<bool>> guard,
            Func<TEvent, TEventParameter, Task> effect)
        {
            Destination = destination;
            Guard = guard ?? EmptyGuard;
            Effect = effect ?? EmptyEffect;
        }

        public TState Destination { get; }

        public Func<TEvent, TEventParameter, Task<bool>> Guard { get; }

        public Func<TEvent, TEventParameter, Task> Effect { get; }

#pragma warning disable SA1313 // ParameterNamesMustBeginWithLowerCaseLetter
        private static readonly Func<TEvent, TEventParameter, Task> EmptyEffect = (_, __) => Task.CompletedTask;

        private static readonly Func<TEvent, TEventParameter, Task<bool>> EmptyGuard = (_, __) => Task.FromResult(true);
#pragma warning restore SA1313 // ParameterNamesMustBeginWithLowerCaseLetter
    }
}
