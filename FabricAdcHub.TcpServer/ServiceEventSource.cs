using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FabricAdcHub.TcpServer
{
    [EventSource(Name = "FabricAdcHub.ServiceFabric-TcpServer")]
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
            public const EventKeywords TcpExchange = (EventKeywords)0x4L;
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

        [Event(TcpExchangeStartedEventId, Level = EventLevel.Verbose, Message = "TCP exchange started with '{0}'", Keywords = Keywords.TcpExchange)]
        public void TcpExchangeStarted(string ipAddress)
        {
            WriteEvent(TcpExchangeStartedEventId, ipAddress);
        }

        [Event(TcpExchangeEndedEventId, Level = EventLevel.Verbose, Message = "TCP exchange ended with '{0}'", Keywords = Keywords.TcpExchange)]
        public void TcpExchangeEnded(string ipAddress)
        {
            WriteEvent(TcpExchangeEndedEventId, ipAddress);
        }

        [Event(TcpExchangeFailedEventId, Level = EventLevel.Error, Message = "TCP exchange failed", Keywords = Keywords.TcpExchange)]
        public void TcpExchangeFailed(string exception)
        {
            WriteEvent(TcpExchangeFailedEventId, exception);
        }

        [Event(AdcMessageReceivedEventId, Level = EventLevel.Verbose, Message = "ADC message received '{0}'", Keywords = Keywords.TcpExchange)]
        public void AdcMessageReceived(string message)
        {
            WriteEvent(AdcMessageReceivedEventId, message);
        }

        [Event(AdcMessageSentEventId, Level = EventLevel.Verbose, Message = "ADC message sent '{0}'", Keywords = Keywords.TcpExchange)]
        public void AdcMessageSent(string message)
        {
            WriteEvent(AdcMessageSentEventId, message);
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

        private const int TcpExchangeStartedEventId = 8;
        private const int TcpExchangeEndedEventId = 9;
        private const int TcpExchangeFailedEventId = 10;
        private const int AdcMessageReceivedEventId = 11;
        private const int AdcMessageSentEventId = 12;

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
