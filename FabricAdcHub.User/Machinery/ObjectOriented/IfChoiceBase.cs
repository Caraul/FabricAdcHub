using System.Threading.Tasks;

namespace FabricAdcHub.User.Machinery.ObjectOriented
{
    public abstract class IfChoiceBase<TState, TEvent, TEventParameter>
    {
        protected IfChoiceBase(TEvent trigger, TState ifDestination, TState elseDestination)
        {
            Trigger = trigger;
            IfDestination = ifDestination;
            ElseDestination = elseDestination;
        }

        public TEvent Trigger { get; }

        public abstract Task<bool> Guard(TEvent evt, TEventParameter parameter);

        public TState IfDestination { get; }

        public abstract Task IfEffect(TEvent evt, TEventParameter parameter);

        public TState ElseDestination { get; }

        public abstract Task ElseEffect(TEvent evt, TEventParameter parameter);
    }
}
