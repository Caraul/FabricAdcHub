using System;
using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;

namespace FabricAdcHub.Core
{
    public static class MessageSerializer
    {
        public const string Separator = " ";

        public static bool TryCreateFromText(string text, out Command command)
        {
            command = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var parts = text.Split(Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var messageTypeAndName = parts[0];
            if (messageTypeAndName.Length != 4)
            {
                return false;
            }

            var parameters = parts.Skip(1).ToList();

            var messageTypeSymbol = messageTypeAndName[0];
            if (!MessageHeaderCreators.ContainsKey(messageTypeSymbol))
            {
                return false;
            }

            var messageHeader = MessageHeaderCreators[messageTypeSymbol](parameters);
            var commandName = messageTypeAndName.Substring(1, 3);
            if (!CommandCreators.ContainsKey(commandName))
            {
                return false;
            }

            command = CommandCreators[commandName](messageHeader, parameters.Skip(messageHeader.Type.NumberOfParameters).ToList());
            return true;
        }

        private static readonly Dictionary<string, Func<MessageHeader, IList<string>, Command>> CommandCreators = new Dictionary<string, Func<MessageHeader, IList<string>, Command>>
        {
            { CommandType.Supports.Name, (messageHeader, parameters) => new Supports(messageHeader, parameters) },
            { CommandType.Status.Name, (messageHeader, parameters) => new Status(messageHeader, parameters) },
            { CommandType.Sid.Name, (messageHeader, parameters) => new Sid(messageHeader, parameters) },
            { CommandType.Information.Name, (messageHeader, parameters) => new Information(messageHeader, parameters) },
            { CommandType.GetPassword.Name, (messageHeader, parameters) => new GetPassword(messageHeader, parameters) },
            { CommandType.ConnectToMe.Name, (messageHeader, parameters) => new ConnectToMe(messageHeader, parameters) },
            { CommandType.ReversedConnectToMe.Name, (messageHeader, parameters) => new ReversedConnectToMe(messageHeader, parameters) },
            { CommandType.Quit.Name, (messageHeader, parameters) => new Quit(messageHeader, parameters) },
            { CommandType.Message.Name, (messageHeader, parameters) => new Msg(messageHeader, parameters) },
            { CommandType.Search.Name, (messageHeader, parameters) => new Search(messageHeader, parameters) },
            { CommandType.Result.Name, (messageHeader, parameters) => new Result(messageHeader, parameters) }
        };

        private static readonly Dictionary<char, Func<IList<string>, MessageHeader>> MessageHeaderCreators = new Dictionary<char, Func<IList<string>, MessageHeader>>
        {
            { MessageHeaderType.Broadcast.Symbol, parameters => new BroadcastMessageHeader(parameters) },
            { MessageHeaderType.Direct.Symbol, parameters => new DirectMessageHeader(parameters) },
            { MessageHeaderType.Echo.Symbol, parameters => new EchoMessageHeader(parameters) },
            { MessageHeaderType.Information.Symbol, _ => new InformationMessageHeader() },
            { MessageHeaderType.FeatureBroadcast.Symbol, parameters => new FeatureBroadcastMessageHeader(parameters) },
            { MessageHeaderType.HubOnly.Symbol, _ => new HubOnlyMessageHeader() },
            { MessageHeaderType.DirectTcp.Symbol, _ => new DirectTcpMessageHeader() },
            { MessageHeaderType.DirectUdp.Symbol, parameters => new DirectUdpMessageHeader(parameters) }
        };
    }
}
