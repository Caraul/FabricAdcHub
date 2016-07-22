using System.Threading.Tasks;

namespace FabricAdcHub.User.Machinery.ObjectOriented
{
    public abstract class TransitionBase<TState, TEvent, TEventParameter>
    {
        protected TransitionBase(TState destination, TEvent trigger)
        {
            Destination = destination;
            Trigger = trigger;
        }

        public TState Destination { get; }

        public TEvent Trigger { get; }

        public abstract Task<bool> Guard(TEvent evt, TEventParameter parameter);

        public abstract Task Effect(TEvent evt, TEventParameter parameter);
    }
}
