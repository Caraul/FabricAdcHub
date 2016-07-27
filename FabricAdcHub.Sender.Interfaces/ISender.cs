using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace FabricAdcHub.Sender.Interfaces
{
    public interface ISender : IActor, IActorEventPublisher<ISenderEvents>
    {
        Task SendMessage(string message);

        Task Close();
    }
}
