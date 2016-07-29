using System;
using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.Commands;
using FabricAdcHub.Core.MessageHeaders;
using FabricAdcHub.Core.Utilites;
using CommandCreator =
    System.Func<
        FabricAdcHub.Core.MessageHeaders.MessageHeader,
        System.Collections.Generic.IList<string>,
        System.Collections.Generic.IList<string>,
        string,
        FabricAdcHub.Core.Commands.Command>;

namespace FabricAdcHub.Core
{
    public static class MessageSerializer
    {
        public static bool TryCreateFromMessage(string message, out Command command)
        {
            command = null;
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            var parts = SplitText(message);
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

            var commandType = CommandType.FromText(commandName);
            var positionalParameters = parameters
                .Skip(messageHeader.Type.NumberOfParameters)
                .Take(commandType.NumberOfParameters)
                .ToList();
            var namedParameters = parameters
                .Skip(messageHeader.Type.NumberOfParameters)
                .Skip(commandType.NumberOfParameters)
                .ToList();
            command = CommandCreators[commandName](messageHeader, positionalParameters, namedParameters, message);
            return true;
        }

        public static string BuildText(IEnumerable<string> parts)
        {
            return BuildText(parts.ToArray());
        }

        public static string BuildText(params string[] parts)
        {
            return string.Join(Separator, parts.Where(part => !string.IsNullOrEmpty(part)));
        }

        public static string[] SplitText(string text)
        {
            return text
                .Split(Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Unescape())
                .ToArray();
        }

        private const string Separator = " ";

        private static readonly Dictionary<string, CommandCreator> CommandCreators = new Dictionary<string, CommandCreator>
        {
            { CommandType.Supports.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Supports(messageHeader, namedParameters, originalMessage) },
            { CommandType.Status.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Status(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.Sid.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Sid(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.Information.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Information(messageHeader, namedParameters, originalMessage) },
            { CommandType.GetPassword.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new GetPassword(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.ConnectToMe.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new ConnectToMe(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.ReversedConnectToMe.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new ReversedConnectToMe(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.Quit.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Quit(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.Message.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Msg(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.Search.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Search(messageHeader, namedParameters, originalMessage) },
            { CommandType.Result.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Result(messageHeader, namedParameters, originalMessage) },
            { CommandType.Password.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Password(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.Get.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Get(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.GetFileInformation.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new GetFileInformation(messageHeader, positionalParameters, namedParameters, originalMessage) },
            { CommandType.Send.Name, (messageHeader, positionalParameters, namedParameters, originalMessage) => new Send(messageHeader, positionalParameters, namedParameters, originalMessage) }
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
