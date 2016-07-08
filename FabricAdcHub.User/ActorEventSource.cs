using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FabricAdcHub.User
{
    [EventSource(Name = "FabricAdcHub.ServiceFabric-FabricAdcHub.User")]
    internal sealed class ActorEventSource : EventSource
    {
        public static readonly ActorEventSource Current = new ActorEventSource();

        static ActorEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { }).Wait();
        }

        public static class Keywords
        {
            public const EventKeywords HostInitialization = (EventKeywords)0x1L;
            public const EventKeywords AdcCommand = (EventKeywords)0x2L;
            public const EventKeywords State = (EventKeywords)0x4L;
            public const EventKeywords OnOff = (EventKeywords)0x8L;
        }

        [NonEvent]
        public void Message(string message, params object[] args)
        {
            if (IsEnabled())
            {
                Message(string.Format(message, args));
            }
        }

        [Event(MessageEventId, Level = EventLevel.Informational, Message = "{0}")]
        public void Message(string message)
        {
            if (IsEnabled())
            {
                WriteEvent(MessageEventId, message);
            }
        }

        [NonEvent]
        public void ActorMessage(Actor actor, string message, params object[] args)
        {
            if (IsEnabled() && actor.Id != null && actor.ActorService?.Context?.CodePackageActivationContext != null)
            {
                ActorMessage(
                    actor.GetType().ToString(),
                    actor.Id.ToString(),
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                    actor.ActorService.Context.ServiceTypeName,
                    actor.ActorService.Context.ServiceName.ToString(),
                    actor.ActorService.Context.PartitionId,
                    actor.ActorService.Context.ReplicaId,
                    actor.ActorService.Context.NodeContext.NodeName,
                    string.Format(message, args));
            }
        }

        [Event(ActorHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Actor host initialization failed", Keywords = Keywords.HostInitialization)]
        public void ActorHostInitializationFailed(string exception)
        {
            WriteEvent(ActorHostInitializationFailedEventId, exception);
        }

        [Event(StateInitializedEventId, Level = EventLevel.Verbose, Message = "State for '{0}' initialized to '{1}'", Keywords = Keywords.State)]
        public void StateInitialized(string sid, string state)
        {
            WriteEvent(StateInitializedEventId, sid, state);
        }

        [Event(StateChangedEventId, Level = EventLevel.Verbose, Message = "State for '{0}' changed from '{1}' to '{2}'", Keywords = Keywords.State)]
        public void StateChanged(string sid, string oldState, string newState)
        {
            WriteEvent(StateChangedEventId, sid, oldState, newState);
        }

        [Event(OpenedEventId, Level = EventLevel.Verbose, Message = "User '{0}' is opened", Keywords = Keywords.OnOff)]
        public void Opened(string sid)
        {
            WriteEvent(OpenedEventId, sid);
        }

        [Event(ClosedEventId, Level = EventLevel.Verbose, Message = "User '{0}' is closed", Keywords = Keywords.OnOff)]
        public void Closed(string sid)
        {
            WriteEvent(ClosedEventId, sid);
        }

        [Event(CommandDeserializationFailedEventId, Level = EventLevel.Warning, Message = "Message '{0}' is not a valid ADC command", Keywords = Keywords.AdcCommand)]
        public void CommandDeserializationFailed(string message)
        {
            WriteEvent(CommandDeserializationFailedEventId, message);
        }

        private ActorEventSource()
        {
        }

        private const int MessageEventId = 1;
        private const int ActorMessageEventId = 2;
        private const int ActorHostInitializationFailedEventId = 3;

        private const int StateInitializedEventId = 4;
        private const int StateChangedEventId = 5;
        private const int OpenedEventId = 6;
        private const int ClosedEventId = 7;
        private const int CommandDeserializationFailedEventId = 8;

        [Event(ActorMessageEventId, Level = EventLevel.Informational, Message = "{9}")]
        private void ActorMessage(
            string actorType,
            string actorId,
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            Guid partitionId,
            long replicaOrInstanceId,
            string nodeName,
            string message)
        {
            WriteEvent(
                ActorMessageEventId,
                actorType,
                actorId,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                message);
        }
    }
}
