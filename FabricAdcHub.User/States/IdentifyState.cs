using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Messages;
using FabricAdcHub.Core.MessageTypes;
using FabricAdcHub.Core.Utilites;
using FabricAdcHub.User.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.User.States
{
    internal class IdentifyState : StateBase
    {
        public IdentifyState(User user)
            : base(user)
        {
        }

        public override State State => State.Identify;

        public override Task<State> ProcessMessage(Message message)
        {
            if (message.MessageName == MessageName.Information)
            {
                return ProcessMessage((InformationMessage)message);
            }

            if (message.MessageName == MessageName.Status)
            {
                return ProcessMessage((StatusMessage)message);
            }

            throw new InvalidCommandException();
        }

        private async Task<State> ProcessMessage(InformationMessage message)
        {
            if (!await CheckClientInformation(message))
            {
                return State;
            }

            var userInformation = await User.UpdateInformation(message);
            var hubInformationMessage = CreateHubInformationMessage();
            await User.SendMessage(hubInformationMessage);

            await BroadcastAllUsersInformation(userInformation);

            return State.Normal;
        }

        private Task<State> ProcessMessage(StatusMessage message)
        {
            return Task.FromResult(State);
        }

        private static InformationMessage CreateHubInformationMessage()
        {
            return new InformationMessage(InformationMessageType)
            {
                ClientType = new NamedParameter<InformationMessage.ClientTypes>(InformationMessage.ClientTypes.Hub),
                Nickname = new NamedParameter<string>("ServiceFabricAdcHub"),
                Description = new NamedParameter<string>("Service Fabric ADC Hub"),
                Features = new NamedParameter<IList<string>>(new[] { "BASE", "TIGR" })
            };
        }

        private async Task<bool> CheckClientInformation(InformationMessage message)
        {
            if (!message.Pid.IsDefined)
            {
                await SendRequiredFieldIsMissing("PD");
                return false;
            }

            if (!message.Cid.IsDefined)
            {
                await SendRequiredFieldIsMissing("ID");
                return false;
            }

            var cid = AdcBase32Encoder.Decode(message.Cid.Value);
            var pid = AdcBase32Encoder.Decode(message.Pid.Value);
            var hash = new TigerHash().ComputeHash(pid);
            if (!hash.SequenceEqual(cid))
            {
                await SendInvalidPid();
                return false;
            }

            if (!message.IpAddressV4.IsDefined)
            {
                await SendRequiredFieldIsMissing("I4");
                return false;
            }

            if (message.IpAddressV4.Value != User.ClientIPv4.ToString())
            {
                await SendInvalidIPv4();
                return false;
            }

            // TODO: check nickname uniqueness
            return true;
        }

        private Task SendRequiredFieldIsMissing(string fieldName)
        {
            var statusMessage = new StatusMessage(
                InformationMessageType,
                StatusMessage.ErrorSeverity.Fatal,
                StatusMessage.ErrorCode.RequiredInfFieldIsMissingOrBad,
                $"Field {fieldName} is required")
            {
                MissingInfField = fieldName
            };
            return User.SendMessage(statusMessage);
        }

        private Task SendInvalidPid()
        {
            var statusMessage = new StatusMessage(
                InformationMessageType,
                StatusMessage.ErrorSeverity.Fatal,
                StatusMessage.ErrorCode.InvalidPidSupplied,
                string.Empty);
            return User.SendMessage(statusMessage);
        }

        private Task SendInvalidIPv4()
        {
            var statusMessage = new StatusMessage(
                InformationMessageType,
                StatusMessage.ErrorSeverity.Fatal,
                StatusMessage.ErrorCode.InvalidIp,
                string.Empty);
            return User.SendMessage(statusMessage);
        }

        private async Task BroadcastAllUsersInformation(string newUserInformation)
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric://FabricAdcHub/Catalog"));
            var allSidsInfo = await catalog.GetAllSidsInformation();
            for (var index = 0; index != allSidsInfo.Length; index++)
            {
                await User.SendSerializedMessage(allSidsInfo[index].Information);
            }

            for (var index = 0; index != allSidsInfo.Length; index++)
            {
                var user = ActorProxy.Create<IUser>(new ActorId(allSidsInfo[index].Sid));
                await user.SendSerializedMessage(newUserInformation);
            }

            await User.SendSerializedMessage(newUserInformation);
        }

        private static readonly InformationMessageType InformationMessageType = new InformationMessageType();
    }
}
