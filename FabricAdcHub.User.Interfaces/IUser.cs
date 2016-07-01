using System.Net;
using System.Threading.Tasks;
using FabricAdcHub.Core.Messages;
using Microsoft.ServiceFabric.Actors;

namespace FabricAdcHub.User.Interfaces
{
    public interface IUser : IActor, IActorEventPublisher<IUserEvents>
    {
        Task<bool> TryStart(IPAddress clientIPv4, IPAddress clientIPv6);

        Task ProcessMessage(string message);

        Task SendMessage(Message message);

        Task SendSerializedMessage(string messageText);
    }
}
