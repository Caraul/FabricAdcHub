using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
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

        public override Task<State> ProcessCommand(Command command)
        {
            if (command.Type == CommandType.Information)
            {
                return ProcessCommand((Information)command);
            }

            if (command.Type == CommandType.Status)
            {
                return ProcessCommand((Status)command);
            }

            throw new InvalidCommandException();
        }

        private async Task<State> ProcessCommand(Information message)
        {
            if (!await CheckClientInformation(message))
            {
                return State;
            }

            var userInformation = await User.UpdateInformation(message);
            var hubInformation = CreateHubInformation();
            await User.SendMessage(hubInformation);

            await BroadcastAllUsersInformation(userInformation);

            return State.Normal;
        }

        private Task<State> ProcessCommand(Status message)
        {
            return Task.FromResult(State);
        }

        private static Information CreateHubInformation()
        {
            var information = new Information(InformationType);
            information.ClientType.Value = Information.ClientTypes.Hub;
            information.Nickname.Value = "ServiceFabricAdcHub";
            information.Description.Value = "Service Fabric ADC Hub";
            information.Features.Value = new HashSet<string> { "BASE", "TIGR" };
            return information;
        }

        private async Task<bool> CheckClientInformation(Information message)
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
            var statusMessage = new Status(
                InformationType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.RequiredInfFieldIsMissingOrBad,
                $"Field {fieldName} is required");
            statusMessage.MissingInfField.Value = fieldName;
            return User.SendMessage(statusMessage);
        }

        private Task SendInvalidPid()
        {
            var statusMessage = new Status(
                InformationType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidPidSupplied,
                string.Empty);
            return User.SendMessage(statusMessage);
        }

        private Task SendInvalidIPv4()
        {
            var statusMessage = new Status(
                InformationType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidIp,
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

        private static readonly InformationMessageHeader InformationType = new InformationMessageHeader();
    }
}
