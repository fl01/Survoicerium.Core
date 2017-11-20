using System;
using System.Security.Cryptography;
using System.Text;

namespace Survoicerium.Core.Hash
{
    public class NameService : INameService
    {
        public const string Entropy = "KQ324A@#!ndg";

        public string GetChannelName(string channelId)
        {
            var stringResult = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes($"{channelId}{Entropy}"));

                foreach (Byte b in result)
                {
                    stringResult.Append(b.ToString("x2"));
                }
            }

            return stringResult.ToString();
        }
    }
}
