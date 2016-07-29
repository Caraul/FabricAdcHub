using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FabricAdcHub.User
{
    [EventSource(Name = "FabricAdcHub.ServiceFabric-User")]
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

        [Event(StateEnteredEventId, Level = EventLevel.Verbose, Message = "State for '{0}' entered to '{1}'", Keywords = Keywords.State)]
        public void StateEntered(string sid, string state)
        {
            WriteEvent(StateEnteredEventId, sid, state);
        }

        [Event(StateExitedEventId, Level = EventLevel.Verbose, Message = "State for '{0}' exited from '{1}'", Keywords = Keywords.State)]
        public void StateExited(string sid, string state)
        {
            WriteEvent(StateExitedEventId, sid, state);
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

        [Event(CommandDeserializationFailedEventId, Level = EventLevel.Warning, Message = "Message '{0}' is an unknown ADC command", Keywords = Keywords.AdcCommand)]
        public void CommandDeserializationFailed(string message)
        {
            WriteEvent(CommandDeserializationFailedEventId, message);
        }

        [Event(NewSidInformationBroadcastedEventId, Level = EventLevel.Informational, Message = "Information is broadcasted '{0}'", Keywords = Keywords.Protocol)]
        public void NewSidInformationBroadcasted(string sid)
        {
            WriteEvent(NewSidInformationBroadcastedEventId, sid);
        }

        [Event(CommandReceivedEventId, Level = EventLevel.Informational, Message = "User '{0}', command received '{1}' ", Keywords = Keywords.Protocol)]
        public void CommandReceived(string sid, string message)
        {
            WriteEvent(CommandReceivedEventId, sid, message);
        }

        [Event(CommandProcessingFailedEventId, Level = EventLevel.Warning, Message = "User '{0}', message '{1}' processing failed with '{2}'", Keywords = Keywords.AdcCommand)]
        public void CommandProcessingFailed(string sid, string message, string exception)
        {
            WriteEvent(CommandProcessingFailedEventId, sid, message, exception);
        }

        private ActorEventSource()
        {
        }

        private const int MessageEventId = 1;
        private const int ActorMessageEventId = 2;
        private const int ActorHostInitializationFailedEventId = 3;

        private const int StateEnteredEventId = 5;
        private const int StateExitedEventId = 6;
        private const int OpenedEventId = 7;
        private const int ClosedEventId = 8;
        private const int CommandDeserializationFailedEventId = 9;
        private const int CommandProcessingFailedEventId = 10;

        private const int NewSidInformationBroadcastedEventId = 13;
        private const int CommandReceivedEventId = 15;

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
