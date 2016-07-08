using Microsoft.ServiceFabric.Actors;

namespace FabricAdcHub.User.Interfaces
{
    public interface IUserEvents : IActorEvents
    {
        void MessageAvailable(string message);

        void Closed();
    }
}
