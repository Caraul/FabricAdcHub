using FabricAdcHub.Core.Commands;
using Xunit;

namespace FabricAdcHub.Core.UnitTests
{
    public class MessageSerializerTests
    {
        [Fact]
        public void FeatureBroadcastMessageHeaderShouldParseCorrectly()
        {
            // arrange
            var originalMessage = "BINF AAAA AW";

            // act
            Command command;
            var result = MessageSerializer.TryCreateFromMessage(originalMessage, out command);
            Assert.True(result);
            var serializedMessage = command.ToMessage();

            // assert
            Assert.Equal(originalMessage, serializedMessage);
        }
    }
}
