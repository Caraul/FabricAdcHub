using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FabricAdcHub.Core.Utilites;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace FabricAdcHub.TcpServer
{
    internal class AdcClients
    {
        public async Task Add(TcpClient tcpClient)
        {
            var sid = await CreateNewUser(tcpClient);
            var adcClient = new AdcClient(tcpClient, sid);
            _adcClients[sid] = adcClient;
            await adcClient.Open();
        }

        private async Task<string> CreateNewUser(TcpClient tcpClient)
        {
            while (true)
            {
                var sid = CreateNewSid();
                var user = ActorProxy.Create<IUser>(new ActorId(sid));
                if (await user.TryStart(
                        ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.MapToIPv4(),
                        ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.MapToIPv6()))
                {
                    return sid;
                }
            }
        }

        private string CreateNewSid()
        {
            var bytes = new byte[3];
            _random.NextBytes(bytes);
            return AdcBase32Encoder.Encode(bytes).Substring(0, 4);
        }

        private readonly Random _random = new Random();
        private readonly Dictionary<string, AdcClient> _adcClients = new Dictionary<string, AdcClient>(StringComparer.InvariantCulture);
    }
}
