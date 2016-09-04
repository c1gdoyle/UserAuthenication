using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Demo.Security.Base;
using Demo.Security.Extensions;

namespace Demo.Security.Ciphers
{
    /// <summary>
    /// An implementation of <see cref="IEncryptionCipher"/> that uses Rijnadel encryption.
    /// </summary>
    public class RijnadelEncryptionCipher : IEncryptionCipher
    {
        private const int KeySize = 256;

        private const int DerivationIterations = 1000;

        public string Encrypt(SecureString input, string key)
        {
            //Salt and IV are randomly generated each time, but are prepended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting

            byte[] saltBytes = Generate256BitsOfRandomEntropy();
            byte[] ivBytes = Generate256BitsOfRandomEntropy();

            return Encrypt(input, key, saltBytes, ivBytes);
        }

        public string Encrypt(SecureString input, string key, byte[] salt, byte[] iv)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input.ToInSecureString());

            using (var password = new Rfc2898DeriveBytes(key, salt, DerivationIterations))
            {
                var keyBytes = password.GetBytes(KeySize / 8);
                using (var rijndael = new RijndaelManaged())
                {
                    rijndael.BlockSize = 256;
                    rijndael.Mode = CipherMode.CBC;
                    rijndael.Padding = PaddingMode.PKCS7;

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream,
                            rijndael.CreateEncryptor(keyBytes, salt),
                            CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            //Create the final bytes as a conatenation of the random salt, IV and Cipher bytes
                            var cipherTextBytes = salt;
                            cipherTextBytes = cipherTextBytes.Concat(salt).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
