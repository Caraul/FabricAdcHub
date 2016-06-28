using System.Linq;
using FabricAdcHub.Core.MessageTypes;
using Xunit;

namespace FabricAdcHub.Core.UnitTests
{
    public class FeatureBroadcastMessageTypeTests
    {
        [Fact]
        public void FeatureBroadcastMessageTypeShouldParseCorrectly()
        {
            // arrange
            var parameters = new[] { "XOXO", "+FEA1-FEA2+FEA3-FEA4-FEA5" };
            var messageType = new FeatureBroadcastMessageType();

            // act
            var skipParameters = messageType.FromText(parameters.ToList());
            var toText = messageType.ToText();

            // assert
            Assert.Equal(2, skipParameters);
            Assert.Equal("XOXO", messageType.Sid);
            Assert.Equal(new[] { "FEA1", "FEA3" }, messageType.RequiredFeatures.ToList());
            Assert.Equal(new[] { "FEA2", "FEA4", "FEA5" }, messageType.ExcludedFeatures.ToList());
            Assert.Equal("XOXO +FEA1+FEA3-FEA2-FEA4-FEA5", toText);
        }
    }
}
