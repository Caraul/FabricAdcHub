namespace FabricAdcHub.User.Machinery
{
    public class Transition<TState, TEvent, TEventParameter>
    {
        public Transition(TState source, TState destination, TEvent trigger, TEventParameter parameter)
        {
            Source = source;
            Destination = destination;
            Trigger = trigger;
            Parameter = parameter;
        }

        public TState Source { get; }

        public TState Destination { get; }

        public TEvent Trigger { get; }

        public TEventParameter Parameter { get; }
    }
}
