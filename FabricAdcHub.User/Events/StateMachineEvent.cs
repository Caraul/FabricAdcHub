using System;
using FabricAdcHub.Core.Commands;

namespace FabricAdcHub.User.Events
{
    internal class StateMachineEvent : IEquatable<StateMachineEvent>
    {
        public static readonly StateMachineEvent ClientOpened = new StateMachineEvent(InternalEvent.ClientOpened);
        public static readonly StateMachineEvent NetworkErrorOccured = new StateMachineEvent(InternalEvent.NetworkErrorOccured);

        public StateMachineEvent(InternalEvent internalEvent)
            : this(internalEvent, null)
        {
        }

        public StateMachineEvent(InternalEvent internalEvent, CommandType commandType)
        {
            InternalEvent = internalEvent;
            CommandType = commandType;
        }

        public InternalEvent InternalEvent { get; }

        public CommandType CommandType { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as StateMachineEvent);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + InternalEvent.GetHashCode();
                if (CommandType != null)
                {
                    hash = (hash * 23) + CommandType.GetHashCode();
                }

                return hash;
            }
        }

        public bool Equals(StateMachineEvent other)
        {
            return
                other != null
                && other.InternalEvent == InternalEvent
                && other.CommandType == CommandType;
        }
    }
}
