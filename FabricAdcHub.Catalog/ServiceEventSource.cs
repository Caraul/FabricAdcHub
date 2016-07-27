using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FabricAdcHub.Catalog
{
    [EventSource(Name = "FabricAdcHub.ServiceFabric-Catalog")]
    internal sealed class ServiceEventSource : EventSource
    {
        public static readonly ServiceEventSource Current = new ServiceEventSource();

        static ServiceEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { }).Wait();
        }

        public static class Keywords
        {
            public const EventKeywords Requests = (EventKeywords)0x1L;
            public const EventKeywords ServiceInitialization = (EventKeywords)0x2L;
            public const EventKeywords Sid = (EventKeywords)0x4L;
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
        public void ServiceMessage(StatelessService service, string message, params object[] args)
        {
            if (IsEnabled())
            {
                ServiceMessage(
                    service.Context.ServiceName.ToString(),
                    service.Context.ServiceTypeName,
                    service.Context.InstanceId,
                    service.Context.PartitionId,
                    service.Context.CodePackageActivationContext.ApplicationName,
                    service.Context.CodePackageActivationContext.ApplicationTypeName,
                    service.Context.NodeContext.NodeName,
                    string.Format(message, args));
            }
        }

        [Event(ServiceTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}", Keywords = Keywords.ServiceInitialization)]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(ServiceTypeRegisteredEventId, hostProcessId, serviceType);
        }

        [Event(ServiceHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Service host initialization failed", Keywords = Keywords.ServiceInitialization)]
        public void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent(ServiceHostInitializationFailedEventId, exception);
        }

        [Event(ServiceRequestStartEventId, Level = EventLevel.Informational, Message = "Service request '{0}' started", Keywords = Keywords.Requests)]
        public void ServiceRequestStart(string requestTypeName)
        {
            WriteEvent(ServiceRequestStartEventId, requestTypeName);
        }

        [Event(ServiceRequestStopEventId, Level = EventLevel.Informational, Message = "Service request '{0}' finished", Keywords = Keywords.Requests)]
        public void ServiceRequestStop(string requestTypeName)
        {
            WriteEvent(ServiceRequestStopEventId, requestTypeName);
        }

        [Event(ServiceRequestFailedEventId, Level = EventLevel.Error, Message = "Service request '{0}' failed", Keywords = Keywords.Requests)]
        public void ServiceRequestFailed(string requestTypeName, string exception)
        {
            WriteEvent(ServiceRequestFailedEventId, exception);
        }

        [Event(SidReservedEventId, Level = EventLevel.Informational, Message = "Sid '{0}' is reserved", Keywords = Keywords.Sid)]
        public void SidReserved(string sid)
        {
            WriteEvent(SidReservedEventId, sid);
        }

        [Event(SidReleasedEventId, Level = EventLevel.Informational, Message = "Sid '{0}' is released", Keywords = Keywords.Sid)]
        public void SidReleased(string sid)
        {
            WriteEvent(SidReleasedEventId, sid);
        }

        [Event(SidReservationFailedEventId, Level = EventLevel.Warning, Message = "Sid cant be reserved due to '{0}'", Keywords = Keywords.Sid)]
        public void SidReservationFailed(string error)
        {
            WriteEvent(SidReservationFailedEventId, error);
        }

        [Event(NewSidInformationBroadcastedEventId, Level = EventLevel.Informational, Message = "Information is broadcasted '{0}'", Keywords = Keywords.Requests)]
        public void NewSidInformationBroadcasted(string sid)
        {
            WriteEvent(NewSidInformationBroadcastedEventId, sid);
        }

        [Event(BroadcastedEventId, Level = EventLevel.Verbose, Message = "Message is broadcasted '{0}'", Keywords = Keywords.Requests)]
        public void Broadcasted(string message)
        {
            WriteEvent(BroadcastedEventId, message);
        }

        [Event(FeatureBroadcastedEventId, Level = EventLevel.Verbose, Message = "Message is feature broadcasted '{0}'", Keywords = Keywords.Requests)]
        public void FeatureBroadcasted(string message)
        {
            WriteEvent(FeatureBroadcastedEventId, message);
        }

        [Event(InformationUpdatedEventId, Level = EventLevel.Verbose, Message = "Information is updated for '{0}'", Keywords = Keywords.Requests)]
        public void InformationUpdated(string sid)
        {
            WriteEvent(InformationUpdatedEventId, sid);
        }

        private ServiceEventSource()
        {
        }

        private const int MessageEventId = 1;
        private const int ServiceMessageEventId = 2;
        private const int ServiceTypeRegisteredEventId = 3;
        private const int ServiceHostInitializationFailedEventId = 4;
        private const int ServiceRequestStartEventId = 5;
        private const int ServiceRequestStopEventId = 6;
        private const int ServiceRequestFailedEventId = 7;

        private const int SidReservedEventId = 10;
        private const int SidReleasedEventId = 11;
        private const int SidReservationFailedEventId = 12;
        private const int NewSidInformationBroadcastedEventId = 13;
        private const int BroadcastedEventId = 14;
        private const int FeatureBroadcastedEventId = 15;
        private const int InformationUpdatedEventId = 16;

        [Event(ServiceMessageEventId, Level = EventLevel.Informational, Message = "{7}")]
        private void ServiceMessage(
            string serviceName,
            string serviceTypeName,
            long replicaOrInstanceId,
            Guid partitionId,
            string applicationName,
            string applicationTypeName,
            string nodeName,
            string message)
        {
            WriteEvent(ServiceMessageEventId, serviceName, serviceTypeName, replicaOrInstanceId, partitionId, applicationName, applicationTypeName, nodeName, message);
        }
    }
}
