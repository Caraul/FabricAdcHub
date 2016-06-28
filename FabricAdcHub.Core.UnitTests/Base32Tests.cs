using Albireo.Base32;
using FabricAdcHub.Core.Utilites;
using Xunit;

namespace FabricAdcHub.Core.UnitTests
{
    public class Base32Tests
    {
        [Fact]
        public void DecodeShouldCorrectlyProcessWithoutEqualSigns()
        {
            // arrange
            var data = new byte[] { 1, 2, 3, 4 };
            var fullBase32 = Base32.Encode(data);
            var trimmedBase32 = AdcBase32Encoder.Encode(data);

            // act
            var decodedData = AdcBase32Encoder.Decode(trimmedBase32);

            // assert
            Assert.NotEqual(fullBase32, trimmedBase32);
            Assert.Equal(data, decodedData);
        }
    }
}
