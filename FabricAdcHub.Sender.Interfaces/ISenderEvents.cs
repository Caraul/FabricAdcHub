using Microsoft.ServiceFabric.Actors;

namespace FabricAdcHub.Sender.Interfaces
{
    public interface ISenderEvents : IActorEvents
    {
        void MessageAvailable(string message);

        void Closed();
    }
}
