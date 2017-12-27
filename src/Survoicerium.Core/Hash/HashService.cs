using System;
using System.Security.Cryptography;
using System.Text;

namespace Survoicerium.Core.Hash
{
    public class HashService : IHashService
    {
        private readonly string _entropy;

        public HashService(string entropy)
        {
            _entropy = entropy;
        }

        public (Guid, string) GenerateChannelIdentifier(string channelData)
        {
            string channelName = GenerateChannelName(channelData);
            Guid channelId = GenerateChannelId(channelName);

            return (channelId, channelName);
        }

        private Guid GenerateChannelId(string channelName)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(channelName);
            byte[] namespaceBytes = Guid.Parse("{9F544D0D-CC69-4F60-B164-92FC7E2B3766}").ToByteArray();
            SwapByteOrder(namespaceBytes);

            byte[] hash;
            using (HashAlgorithm algorithm = SHA1.Create())
            {
                algorithm.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, null, 0);
                algorithm.TransformFinalBlock(bytes, 0, bytes.Length);
                hash = algorithm.Hash;
                byte[] newGuid = new byte[16];
                Array.Copy(hash, 0, newGuid, 0, 16);
                newGuid[6] = (byte)((newGuid[6] & 0x0F) | (5 << 4));
                newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);
                SwapByteOrder(newGuid);
                return new Guid(newGuid);
            }
        }

        private void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        private void SwapBytes(byte[] guid, int left, int right)
        {
            byte temp = guid[left];
            guid[left] = guid[right];
            guid[right] = temp;
        }

        private string GenerateChannelName(string channelId)
        {
            var stringResult = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes($"{channelId}{_entropy}"));

                foreach (Byte b in result)
                {
                    stringResult.Append(b.ToString("x2"));
                }
            }

            return stringResult.ToString();
        }
    }
}
