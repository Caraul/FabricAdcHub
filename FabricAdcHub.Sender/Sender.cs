using System.Threading.Tasks;
using FabricAdcHub.Sender.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FabricAdcHub.Sender
{
    [StatePersistence(StatePersistence.None)]
    internal class Sender : Actor, ISender
    {
        public Sender(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task SendMessage(string message)
        {
            var events = GetEvent<ISenderEvents>();
            events.MessageAvailable(message);
            ActorEventSource.Current.CommandSent(Id.GetStringId(), message);
            return Task.CompletedTask;
        }

        public Task Close()
        {
            var events = GetEvent<ISenderEvents>();
            events.Closed();
            return Task.CompletedTask;
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
        }
    }
}