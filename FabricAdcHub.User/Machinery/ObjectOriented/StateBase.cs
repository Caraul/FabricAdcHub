using System.Threading.Tasks;

namespace FabricAdcHub.User.Machinery.ObjectOriented
{
    public abstract class StateBase<TState, TEvent, TEventParameter>
    {
        public TState State { get; }

        public abstract Task OnEntry(Transition<TState, TEvent, TEventParameter> transition);

        public abstract Task OnExit(Transition<TState, TEvent, TEventParameter> transition);

        protected StateBase(TState state)
        {
            State = state;
        }
    }
}
