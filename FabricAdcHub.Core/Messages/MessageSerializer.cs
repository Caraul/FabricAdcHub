using System;
using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.MessageTypes;

namespace FabricAdcHub.Core.Messages
{
    public static class MessageSerializer
    {
        public static Message FromText(string text)
        {
            var parts = text.Split(Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var messageId = parts[0];
            var parameters = parts.Skip(1).ToList();

            var messageTypeText = messageId[0];
            var messageTypeName = MessageTypeName.FromText(messageTypeText);
            var messageType = MessageTypeCreators[messageTypeName.MessageTypeSymbol](parameters);
            var messageTypeParametersCount = messageType.FromText(parameters);
            var messageNameText = messageId.Substring(1, 3);
            var messageName = MessageName.FromText(messageNameText);
            var message = MessageCreators[messageName.MessageNameText](messageType);

            message.FromText(parameters.Skip(messageTypeParametersCount).ToList());
            return message;
        }

        public const string Separator = " ";

        private static readonly Dictionary<string, Func<MessageType, Message>> MessageCreators = new Dictionary<string, Func<MessageType, Message>>
        {
            { MessageName.Supports.MessageNameText, messageType => new SupportsMessage(messageType) },
            { MessageName.Status.MessageNameText, messageType => new StatusMessage(messageType) },
            { MessageName.Sid.MessageNameText, messageType => new SidMessage(messageType) },
            { MessageName.Information.MessageNameText, messageType => new InformationMessage(messageType) },
            { MessageName.GetPassword.MessageNameText, messageType => new GetPasswordMessage(messageType) },
            { MessageName.ConnectToMe.MessageNameText, messageType => new ConnectToMeMessage(messageType) },
            { MessageName.ReversedConnectToMe.MessageNameText, messageType => new ReverseConnectToMeMessage(messageType) },
            { MessageName.Quit.MessageNameText, messageType => new QuitMessage(messageType) },
            { MessageName.Message.MessageNameText, messageType => new MsgMessage(messageType) },
            { MessageName.Search.MessageNameText, messageType => new SearchMessage(messageType) },
            { MessageName.Result.MessageNameText, messageType => new ResultMessage(messageType) }
        };

        private static readonly Dictionary<char, Func<IList<string>, MessageType>> MessageTypeCreators = new Dictionary<char, Func<IList<string>, MessageType>>
        {
            { MessageTypeName.Broadcast.MessageTypeSymbol, messageType => new BroadcastMessageType() },
            { MessageTypeName.DirectFromHub.MessageTypeSymbol, messageType => new DirectFromHubMessageType() },
            { MessageTypeName.Echo.MessageTypeSymbol, messageType => new EchoMessageType() },
            { MessageTypeName.Information.MessageTypeSymbol, messageType => new InformationMessageType() },
            { MessageTypeName.FeatureBroadcast.MessageTypeSymbol, messageType => new FeatureBroadcastMessageType() },
            { MessageTypeName.ForHub.MessageTypeSymbol, messageType => new ForHubMessageType() },
            { MessageTypeName.DirectTcp.MessageTypeSymbol, messageType => new DirectTcpMessageType() },
            { MessageTypeName.DirectUdp.MessageTypeSymbol, messageType => new DirectUdpMessageType() }
        };
    }
}
