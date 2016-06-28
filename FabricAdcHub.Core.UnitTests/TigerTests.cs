using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FabricAdcHub.Core.Utilites;
using Xunit;

namespace FabricAdcHub.Core.UnitTests
{
    public class TigerTests
    {
        [Fact]
        public void TigerShouldBeCorrectForAllTestData()
        {
            foreach (var pair in TestData)
            {
                // arrange
                var tiger = new TigerHash();

                // act
                var hash = tiger.ComputeHash(Encoding.ASCII.GetBytes(pair.Key));

                // assert
                Assert.Equal(pair.Value, HashToString(hash), true);
            }
        }

        [Fact]
        public void TigerShouldBeCorrectForTestDataSet1()
        {
            TestDataSet1.Add(new string('a', 1000000), "6DB0E2729CBEAD93D715C6A7D36302E9B3CEE0D2BC314B41");
            foreach (var pair in TestDataSet1)
            {
                // arrange
                var tiger = new TigerHash();

                // act
                var hash = tiger.ComputeHash(Encoding.ASCII.GetBytes(pair.Key));

                // assert
                Assert.Equal(pair.Value, HashToString(hash), true);
            }
        }

        [Fact]
        public void TigerShouldBeCorrectForTestDataSet2()
        {
            var correctHashes = File.ReadAllLines("TestSet2Data.txt");
            foreach (var byteNum in Enumerable.Range(0, 128))
            {
                // arrange
                var data = new byte[byteNum];
                var tiger = new TigerHash();

                // act
                var hash = tiger.ComputeHash(data);

                // assert
                Assert.Equal(correctHashes[byteNum * 8], HashToString(hash), true);
            }
        }

        [Fact]
        public void TigerShouldBeCorrectForTestDataSet3()
        {
            var oneBitBytes = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };
            var correctHashes = File.ReadAllLines("TestSet3Data.txt");
            foreach (var byteIndex in Enumerable.Range(0, 64))
            {
                foreach (var byteValue in oneBitBytes)
                {
                    // arrange
                    var data = new byte[64];
                    data[byteIndex] = byteValue;
                    var tiger = new TigerHash();

                    // act
                    var hash = tiger.ComputeHash(data);

                    // assert
                    Assert.Equal(correctHashes[(byteIndex * 8) + Array.IndexOf(oneBitBytes, byteValue)], HashToString(hash), true);
                }
            }
        }

        [Fact]
        public void TigerShouldBeCorrectForTestDataSet4()
        {
            var data = new byte[24];

            // arrange
            var tiger = new TigerHash();

            // act
            var hash = tiger.ComputeHash(data);

            // assert
            Assert.Equal("CDDDCACFEA7B70B485655BA3DC3F60DEE4F6B8F861069E33", HashToString(hash), true);

            data = Enumerable
                .Range(0, 100000)
                .Select(iteration => new TigerHash())
                .Aggregate(data, (current, newTiger) => newTiger.ComputeHash(current));

            // assert
            Assert.Equal("35C4F594F7E827FFC68BFECEBEDA314EDC6FE917BDF00B66", HashToString(data), true);
        }

        private static string HashToString(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        private static readonly Dictionary<string, string> TestData = new Dictionary<string, string>
        {
            { string.Empty, "3293AC630C13F0245F92BBB1766E16167A4E58492DDE73F3" },
            { "a", "77BEFBEF2E7EF8AB2EC8F93BF587A7FC613E247F5F247809" },
            { "abc", "2AAB1484E8C158F2BFB8C5FF41B57A525129131C957B5F93" },
            { "Tiger", "DD00230799F5009FEC6DEBC838BB6A27DF2B9D6F110C7937" },
            { "The quick brown fox jumps over the lazy dog", "6D12A41E72E644F017B6F0E2F7B44C6285F06DD5D2C5B075" },
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-", "F71C8583902AFB879EDFE610F82C0D4786A3A534504486B5" },
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZ=abcdefghijklmnopqrstuvwxyz+0123456789", "48CEEB6308B87D46E95D656112CDF18D97915F9765658957" },
            { "Tiger - A Fast New Hash Function, by Ross Anderson and Eli Biham", "8A866829040A410C729AD23F5ADA711603B3CDD357E4C15E" },
            { "Tiger - A Fast New Hash Function, by Ross Anderson and Eli Biham, proceedings of Fast Software Encryption 3, Cambridge.", "CE55A6AFD591F5EBAC547FF84F89227F9331DAB0B611C889" },
            { "Tiger - A Fast New Hash Function, by Ross Anderson and Eli Biham, proceedings of Fast Software Encryption 3, Cambridge, 1996.", "631abdd103eb9a3d245b6dfd4d77b257fc7439501d1568dd" },
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-", "C54034E5B43EB8005848A7E0AE6AAC76E4FF590AE715FD25" },
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "8DCEA680A17583EE502BA38A3C368651890FFBCCDC49A8CC" },
            { "message digest", "D981F8CB78201A950DCF3048751E441C517FCA1AA55A29F6" },
            { "abcdefghijklmnopqrstuvwxyz", "1714A472EEE57D30040412BFCC55032A0B11602FF37BEEE9" },
            { "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", "0F7BF9A19B9C58F2B7610DF7E84F0AC3A71C631E7B53F78E" },
            { "12345678901234567890123456789012345678901234567890123456789012345678901234567890", "1C14795529FD9F207A958F84C52F11E887FA0CABDFD91BFD" }
        };

        //// http://www.cs.technion.ac.il/~biham/Reports/Tiger/test-vectors-nessie-format.dat
        private static readonly Dictionary<string, string> TestDataSet1 = new Dictionary<string, string>
        {
            { string.Empty, "3293AC630C13F0245F92BBB1766E16167A4E58492DDE73F3" },
            { "a", "77BEFBEF2E7EF8AB2EC8F93BF587A7FC613E247F5F247809" },
            { "abc", "2AAB1484E8C158F2BFB8C5FF41B57A525129131C957B5F93" },
            { "message digest", "D981F8CB78201A950DCF3048751E441C517FCA1AA55A29F6" },
            { "abcdefghijklmnopqrstuvwxyz", "1714A472EEE57D30040412BFCC55032A0B11602FF37BEEE9" },
            { "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", "0F7BF9A19B9C58F2B7610DF7E84F0AC3A71C631E7B53F78E" },
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "8DCEA680A17583EE502BA38A3C368651890FFBCCDC49A8CC" },
            { "12345678901234567890123456789012345678901234567890123456789012345678901234567890", "1C14795529FD9F207A958F84C52F11E887FA0CABDFD91BFD" }
        };
    }
}
