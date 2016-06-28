using System.Collections.Generic;
using System.IO;
using System.Text;
using FabricAdcHub.Core.Utilites;
using Xunit;

namespace FabricAdcHub.Core.UnitTests
{
    public class TigerTreeHashTests
    {
        [Fact]
        public void TthShouldBeCorrectForEmptyString()
        {
            // arrange
            var data = new MemoryStream();

            // act
            var hash = TigerTreeHash.ComputeTth(data);
            var tth = AdcBase32Encoder.Encode(hash);

            // assert
            Assert.Equal("LWPNACQDBZRYXW3VHJVCJ64QBZNGHOHHHZWCLNQ", tth);
        }

        [Fact]
        public void TthShouldBeCorrectForOneZeroByte()
        {
            // arrange
            var zeroByte = new byte[] { 0 };
            var data = new MemoryStream(zeroByte);

            // act
            var hash = TigerTreeHash.ComputeTth(data);
            var tth = AdcBase32Encoder.Encode(hash);

            // assert
            Assert.Equal("VK54ZIEEVTWNAUI5D5RDFIL37LX2IQNSTAXFKSA", tth);
        }

        [Fact]
        public void TthShouldBeCorrectFor1024A()
        {
            // arrange
            var stringData = new string('A', 1024);
            var data = new MemoryStream(Encoding.ASCII.GetBytes(stringData));

            // act
            var hash = TigerTreeHash.ComputeTth(data);
            var tth = AdcBase32Encoder.Encode(hash);

            // assert
            Assert.Equal("L66Q4YVNAFWVS23X2HJIRA5ZJ7WXR3F26RSASFA", tth);
        }

        [Fact]
        public void TthShouldBeCorrectFor1025A()
        {
            // arrange
            var stringData = new string('A', 1025);
            var data = new MemoryStream(Encoding.ASCII.GetBytes(stringData));

            // act
            var hash = TigerTreeHash.ComputeTth(data);
            var tth = AdcBase32Encoder.Encode(hash);

            // assert
            Assert.Equal("PZMRYHGY6LTBEH63ZWAHDORHSYTLO4LEFUIKHWY", tth);
        }

        [Fact]
        public void TthShouldBeCorrectForAllTestData()
        {
            foreach (var pair in TestData)
            {
                // arrange
                var data = new MemoryStream(Encoding.ASCII.GetBytes(pair.Key));

                // act
                var hash = TigerTreeHash.ComputeTth(data);
                var tth = AdcBase32Encoder.Encode(hash);

                // assert
                Assert.Equal(pair.Value, tth);
            }
        }

        private static readonly Dictionary<string, string> TestData = new Dictionary<string, string>
        {
            { string.Empty, "LWPNACQDBZRYXW3VHJVCJ64QBZNGHOHHHZWCLNQ" },
            { "a", "CZQUWH3IYXBF5L3BGYUGZHASSMXU647IP2IKE4Y" },
            { "abc", "ASD4UJSEH5M47PDYB46KBTSQTSGDKLBHYXOMUIA" },
            { "message digest", "YM432MSOX5QILIH2L4TNO62E3O35WYGWSBSJOBA" },
            { "abcdefghijklmnopqrstuvwxyz", "LMHNA2VYO465P2RDOGTR2CL6XKHZNI2X4CCUY5Y" },
            { "The quick brown fox jumps over the lazy dog", "WLM2MITXFTCQXEOYO3M4EL5APES353NQLI66ORY" },
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "TF74ENF7MF2WPDE35M23NRSVKJIRKYRMTLWAHWQ" },
            { "12345678901234567890123456789012345678901234567890123456789012345678901234567890", "NBKCANQ2ODNTSV4C7YJFF3JRAV7LKTFIPHQNBJY" }
        };
    }
}
