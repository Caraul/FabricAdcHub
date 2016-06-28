using System;
using System.Collections.Generic;
using System.IO;

namespace FabricAdcHub.Core.Utilites
{
    public static class TigerTreeHash
    {
        public static byte[] ComputeTth(Stream data)
        {
            if (data.Length == 0)
            {
                var tiger = new TigerHash();
                return tiger.ComputeHash(new byte[] { 0 });
            }

            if (data.Length <= BlockSize)
            {
                return ComputeOneBlockTth(data);
            }

            var leafHashes = LoadLeafHash(data);
            return GetRootHash(leafHashes);
        }

        private static byte[] ComputeOneBlockTth(Stream data)
        {
            var block = ReadNextBlock(data);
            return ComputeLeafHash(block);
        }

        private static IEnumerable<byte[]> LoadLeafHash(Stream data)
        {
            var leafCount = data.Length / BlockSize;
            if (data.Length - (leafCount * BlockSize) > 0)
            {
                leafCount++;
            }

            var leafHashes = new List<byte[]>();
            for (var i = 0; i < leafCount / 2; i++)
            {
                var blockA = ReadNextBlock(data);
                var blockB = ReadNextBlock(data);
                blockA = ComputeLeafHash(blockA);
                blockB = ComputeLeafHash(blockB);
                leafHashes.Add(ComputeInternalHash(blockA, blockB));
            }

            // leaf without a pair.
            if (leafCount % 2 != 0)
            {
                var block = ReadNextBlock(data);
                leafHashes.Add(ComputeLeafHash(block));
            }

            return leafHashes;
        }

        private static byte[] GetRootHash(IEnumerable<byte[]> leafHashes)
        {
            var upLevelHashes = new List<byte[]>(leafHashes);
            do
            {
                var currentHashes = new List<byte[]>(upLevelHashes);
                upLevelHashes.Clear();

                while (currentHashes.Count > 1)
                {
                    // load next two leafs.
                    var hashA = currentHashes[0];
                    var hashB = currentHashes[1];

                    // add their combined hash.
                    upLevelHashes.Add(ComputeInternalHash(hashA, hashB));

                    // remove the used leafs.
                    currentHashes.RemoveAt(0);
                    currentHashes.RemoveAt(0);
                }

                // if this leaf can't combine add him at the end.
                if (currentHashes.Count > 0)
                {
                    upLevelHashes.Add(currentHashes[0]);
                }
            }
            while (upLevelHashes.Count > 1);
            return upLevelHashes[0];
        }

        private static byte[] ReadNextBlock(Stream data)
        {
            var block = new byte[BlockSize];
            var read = data.Read(block, 0, BlockSize);
            Array.Resize(ref block, read);
            return block;
        }

        private static byte[] ComputeInternalHash(byte[] leafA, byte[] leafB)
        {
            var data = new byte[leafA.Length + leafB.Length + 1];
            data[0] = InternalHashMark;
            leafA.CopyTo(data, 1);
            leafB.CopyTo(data, leafA.Length + 1);
            var tiger = new TigerHash();
            return tiger.ComputeHash(data);
        }

        private static byte[] ComputeLeafHash(byte[] rawData)
        {
            var data = new byte[rawData.Length + 1];
            data[0] = LeafHashMark;
            rawData.CopyTo(data, 1);
            var tiger = new TigerHash();
            return tiger.ComputeHash(data);
        }

        private const int BlockSize = 1024;
        private const byte LeafHashMark = 0;
        private const byte InternalHashMark = 1;
    }
}
