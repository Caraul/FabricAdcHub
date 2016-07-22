using FabricAdcHub.Core.Commands;

namespace FabricAdcHub.User.States
{
    internal class StateMachineEvent
    {
        public StateMachineEvent(InternalEvent internalEvent)
            : this(internalEvent, null)
        {
        }

        public StateMachineEvent(InternalEvent internalEvent, Command command)
        {
            InternalEvent = internalEvent;
            Command = command;
        }

        public InternalEvent InternalEvent { get; }

        public Command Command { get; }
    }
}
