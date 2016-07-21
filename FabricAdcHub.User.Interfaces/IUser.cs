using System.Net;
using System.Threading.Tasks;
using FabricAdcHub.Core.Commands;
using Microsoft.ServiceFabric.Actors;

namespace FabricAdcHub.User.Interfaces
{
    public interface IUser : IActor, IActorEventPublisher<IUserEvents>
    {
        Task Open(IPAddress clientIPv4, IPAddress clientIPv6);

        Task Close();

        Task<string> GetInformationMessage();

        Task ProcessMessage(string message);

        Task SendMessage(string message);

        Task Disconnect(DisconnectReason reason);
    }
}
