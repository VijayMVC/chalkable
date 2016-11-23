using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Common
{
    public static class EncryptionTools
    {
        public static string AesEncrypt(string data, string key)
        {
            var aesProvider = new AesCryptoServiceProvider
            {
                KeySize = 256,
                BlockSize = 128,
                Key = key.Substring(0, key.Length/2).HexToByteArray(),
                IV = key.Substring(key.Length/2, key.Length/2).HexToByteArray()
            };

            byte[] inBlock = Encoding.UTF8.GetBytes(data);

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, aesProvider.CreateEncryptor(),
                    CryptoStreamMode.Write))
                {

                    cryptoStream.Write(inBlock, 0, inBlock.Length);
                    cryptoStream.FlushFinalBlock();

                    byte[] outBytes = memoryStream.ToArray();

                    return Convert.ToBase64String(outBytes);
                }
            }
        }

        public static string AesDecrypt(string data, string key)
        {
            var inBytes = Convert.FromBase64String(data);

            var aesProvider = new AesCryptoServiceProvider
            {
                KeySize = 256,
                BlockSize = 128,
                Key = key.Substring(0, key.Length / 2).HexToByteArray(),
                IV = key.Substring(key.Length / 2, key.Length / 2).HexToByteArray()
            };

            using (var memoryStream = new MemoryStream(inBytes))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, aesProvider.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        var plainText = reader.ReadToEnd();

                        return plainText;
                    }
                }
            }
        }
    }
}
