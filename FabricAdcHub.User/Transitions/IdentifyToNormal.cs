using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FabricAdcHub.Catalog.Interfaces;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;
using FabricAdcHub.User.Events;
using FabricAdcHub.User.Machinery.ObjectOriented;
using FabricAdcHub.User.States;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace FabricAdcHub.User.Transitions
{
    internal class IdentifyToNormal : IfChoiceBase<AdcProtocolState, StateMachineEvent, Command>
    {
        public IdentifyToNormal(User user)
            : base(
                new StateMachineEvent(InternalEvent.AdcMessageReceived, CommandType.Information),
                AdcProtocolState.Normal,
                AdcProtocolState.Unknown)
        {
            _user = user;
        }

        public override async Task<bool> Guard(StateMachineEvent evt, Command parameter)
        {
            var command = (Information)parameter;
            if (!command.Pid.IsDefined)
            {
                CreateRequiredFieldIsMissing("PD");
                return false;
            }

            if (!command.Cid.IsDefined)
            {
                CreateRequiredFieldIsMissing("ID");
                return false;
            }

            var cid = AdcBase32Encoder.Decode(command.Cid.Value);
            var pid = AdcBase32Encoder.Decode(command.Pid.Value);
            var hash = new TigerHash().ComputeHash(pid);
            if (!hash.SequenceEqual(cid))
            {
                CreateInvalidPid();
                return false;
            }

            if (!command.IpAddressV4.IsDefined && !command.IpAddressV6.IsDefined)
            {
                CreateRequiredFieldIsMissing("I4");
                return false;
            }

            if (command.IpAddressV4.IsDefined && command.IpAddressV4.Value != _user.ClientIPv4.ToString())
            {
                CreateInvalidIPv4(_user.ClientIPv4.ToString());
                return false;
            }

            if (command.IpAddressV6.IsDefined && command.IpAddressV6.Value != _user.ClientIPv6.ToString())
            {
                CreateInvalidIPv6(_user.ClientIPv6.ToString());
                return false;
            }

            if (!command.Nickname.IsDefined || string.IsNullOrWhiteSpace(command.Nickname.Value))
            {
                CreateInvalidNick();
                return false;
            }

            var catalog = ServiceProxy.Create<ICatalog>(new Uri("fabric:/FabricAdcHub.ServiceFabric/Catalog"));
            if (!await catalog.ReserveNick(_user.Sid, command.Nickname.Value))
            {
                CreateNonUniqueNick();
                return false;
            }

            return true;
        }

        public override async Task IfEffect(StateMachineEvent evt, Command parameter)
        {
            var command = (Information)parameter;
            await _user.UpdateInformation(command);
            var hubInformation = CreateHubInformation();
            await _user.SendCommand(hubInformation);
        }

        public override async Task ElseEffect(StateMachineEvent evt, Command parameter)
        {
            await _user.SendCommand(_errorCommand);
            var quit = new Quit(new InformationMessageHeader(), _user.Sid);
            await _user.SendCommand(quit);
        }

        private void CreateRequiredFieldIsMissing(string fieldName)
        {
            var status = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.RequiredInfFieldIsMissingOrBad,
                $"Field {fieldName} is required");
            status.MissingInfField.Value = fieldName;
            _errorCommand = status;
        }

        private void CreateInvalidPid()
        {
            _errorCommand = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidPidSupplied,
                string.Empty);
        }

        private void CreateInvalidIPv4(string correctIp)
        {
            var status = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidIp,
                string.Empty);
            status.InvalidInfIpv4.Value = correctIp;
            _errorCommand = status;
        }

        private void CreateInvalidIPv6(string correctIp)
        {
            var status = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.InvalidIp,
                string.Empty);
            status.InvalidInfIpv6.Value = correctIp;
            _errorCommand = status;
        }

        private void CreateInvalidNick()
        {
            _errorCommand = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.NickInvalid,
                string.Empty);
        }

        private void CreateNonUniqueNick()
        {
            _errorCommand = new Status(
                InformationMessageHeader,
                Status.ErrorSeverity.Fatal,
                Status.ErrorCode.NickTaken,
                string.Empty);
        }

        private static Information CreateHubInformation()
        {
            var information = new Information(InformationMessageHeader);
            information.ClientType.Value = Information.ClientTypes.Hub;
            information.Nickname.Value = "ServiceFabricAdcHub";
            information.Description.Value = "Service Fabric ADC Hub";
            information.Features.Value = new HashSet<string>(Features);
            return information;
        }

        private static readonly string[] Features = { "BASE", "TIGR" };
        private static readonly InformationMessageHeader InformationMessageHeader = new InformationMessageHeader();
        private readonly User _user;
        private Command _errorCommand;
    }
}
