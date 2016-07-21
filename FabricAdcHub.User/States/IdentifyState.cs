using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;
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

        public override bool IsSendCommandAllowed(Command command)
        {
            return command.Type == CommandType.Status
                || command.Type == CommandType.Information
                || command.Type == CommandType.Quit;
        }

        public override Task<State> ProcessCommand(Command command)
        {
            if (command.Type == CommandType.Information)
            {
                return ProcessInformation((Information)command);
            }

            if (command.Type == CommandType.Status)
            {
                return ProcessStatus((Status)command);
            }

            throw new InvalidCommandException();
        }

        private async Task<State> ProcessInformation(Information command)
        {
            if (!await CheckClientInformation(command))
            {
                return State;
            }

            await User.UpdateInformation(command);
            var hubInformation = CreateHubInformation();
            await User.SendCommand(hubInformation);

            await BroadcastAllUsersInformation();

            return State.Normal;
        }

        private Task<State> ProcessStatus(Status command)
        {
            return Task.FromResult(command.Severity == Status.ErrorSeverity.Fatal ? State.DisconnectedOnProtocolError : State);
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

        private async Task<bool> CheckClientInformation(Information command)
        {
            if (!command.Pid.IsDefined)
            {
                await SendRequiredFieldIsMissing("PD");
                return false;
            }

            if (!command.Cid.IsDefined)
            {
                await SendRequiredFieldIsMissing("ID");
                return false;
            }

            var cid = AdcBase32Encoder.Decode(command.Cid.Value);
            var pid = AdcBase32Encoder.Decode(command.Pid.Value);
            var hash = new TigerHash().ComputeHash(pid);
            if (!hash.SequenceEqual(cid))
            {
                await SendInvalidPid();
                return false;
            }

            if (!command.IpAddressV4.IsDefined && !command.IpAddressV6.IsDefined)
            {
                await SendRequiredFieldIsMissing("I4");
                return false;
            }

            if (command.IpAddressV4.IsDefined && command.IpAddressV4.Value != User.ClientIPv4.ToString())
            {
                await SendInvalidIPv4(User.ClientIPv4.ToString());
                return false;
            }

            if (command.IpAddressV6.IsDefined && command.IpAddressV6.Value != User.ClientIPv6.ToString())
            {
                await SendInvalidIPv6(User.ClientIPv6.ToString());
                return false;
            }

            // TODO: check nickname uniqueness
            return true;
        }

        private Task SendRequiredFieldIsMissing(string fieldName)
        {
            var status = new Status(
                InformationType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.RequiredInfFieldIsMissingOrBad,
                $"Field {fieldName} is required");
            status.MissingInfField.Value = fieldName;
            return User.SendCommand(status);
        }

        private Task SendInvalidPid()
        {
            var status = new Status(
                InformationType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidPidSupplied,
                string.Empty);
            return User.SendCommand(status);
        }

        private Task SendInvalidIPv4(string correctIp)
        {
            var status = new Status(
                InformationType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidIp,
                string.Empty);
            status.InvalidInfIpv4.Value = correctIp;
            return User.SendCommand(status);
        }

        private Task SendInvalidIPv6(string correctIp)
        {
            var status = new Status(
                InformationType,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidIp,
                string.Empty);
            status.InvalidInfIpv6.Value = correctIp;
            return User.SendCommand(status);
        }

        private async Task BroadcastAllUsersInformation()
        {
            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"));
            var newUserInformation = await User.GetInformationMessage();
            await catalog.BroadcastNewSidInformation(User.Sid, newUserInformation);
        }

        private static readonly InformationMessageHeader InformationType = new InformationMessageHeader();
    }
}
