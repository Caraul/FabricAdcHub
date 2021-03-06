﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace FabricAdcHub.User.Interfaces
{
    public interface IUser : IActor
    {
        Task Open(IPAddress clientIPv4, IPAddress clientIPv6);

        Task Close();

        Task<string> GetInformationMessage();

        Task ProcessMessage(string message);

        Task CloseOnDisconnect();
    }
}
