using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FabricAdcHub.Sender
{
    [EventSource(Name = "FabricAdcHub.ServiceFabric-Sender")]
    internal sealed class ActorEventSource : EventSource
    {
        public static readonly ActorEventSource Current = new ActorEventSource();

        static ActorEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { });
        }

        public static class Keywords
        {
            public const EventKeywords HostInitialization = (EventKeywords)0x1L;
            public const EventKeywords Protocol = (EventKeywords)0x10L;
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
        public void ActorMessage(ActorBase actor, string message, params object[] args)
        {
            if (IsEnabled() && actor.Id != null && actor.ActorService?.Context?.CodePackageActivationContext != null)
            {
                string finalMessage = string.Format(message, args);
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
                    finalMessage);
            }
        }

        [Event(ActorHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Actor host initialization failed", Keywords = Keywords.HostInitialization)]
        public void ActorHostInitializationFailed(string exception)
        {
            WriteEvent(ActorHostInitializationFailedEventId, exception);
        }

        [Event(CommandSentEventId, Level = EventLevel.Informational, Message = "User '{0}', command sent '{1}' ", Keywords = Keywords.Protocol)]
        public void CommandSent(string sid, string message)
        {
            WriteEvent(CommandSentEventId, sid, message);
        }

        private const int MessageEventId = 1;
        private const int ActorMessageEventId = 2;
        private const int ActorHostInitializationFailedEventId = 3;

        private const int CommandSentEventId = 14;

        [Event(ActorMessageEventId, Level = EventLevel.Informational, Message = "{9}")]
        private
            void ActorMessage(
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
