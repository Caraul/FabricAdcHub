using System;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.User.States
{
    internal class NormalState : StateBase
    {
        public NormalState(User user)
            : base(AdcProtocolState.Normal, user)
        {
        }

        public override async Task OnEntry(Transition<AdcProtocolState, StateMachineEvent, Command> transition)
        {
            await BroadcastAllUsersInformation();
            await base.OnEntry(transition);
        }

        private async Task BroadcastAllUsersInformation()
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"));
            var newUserInformation = await User.GetInformationMessage();
            await catalog.ExposeSid(User.Sid, newUserInformation);

            ActorEventSource.Current.NewSidInformationBroadcasted(User.Sid);
        }
    }
}
