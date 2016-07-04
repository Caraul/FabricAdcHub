using System.Linq;
using FabricAdcHub.Core.MessageHeaders;
using Xunit;

namespace FabricAdcHub.Core.UnitTests
{
    public class FeatureBroadcastMessageHeaderTests
    {
        [Fact]
        public void FeatureBroadcastMessageHeaderShouldParseCorrectly()
        {
            // arrange
            var parameters = new[] { "XOXO", "+FEA1-FEA2+FEA3-FEA4-FEA5" };
            var messageHeader = new FeatureBroadcastMessageHeader(parameters);

            // act
            var skipParameters = messageHeader.Type.NumberOfParameters;
            var toText = messageHeader.ToText();

            // assert
            Assert.Equal(2, skipParameters);
            Assert.Equal("XOXO", messageHeader.Sid);
            Assert.Equal(new[] { "FEA1", "FEA3" }, messageHeader.RequiredFeatures.ToList());
            Assert.Equal(new[] { "FEA2", "FEA4", "FEA5" }, messageHeader.ExcludedFeatures.ToList());
            Assert.Equal("XOXO +FEA1+FEA3-FEA2-FEA4-FEA5", toText);
        }
    }
}
