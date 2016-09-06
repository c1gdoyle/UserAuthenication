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

        /// <summary>
        /// Encrypts a given <see cref="SecureString"/> input using a supplied key.
        /// </summary>
        /// <param name="input">The input to be encrypted.</param>
        /// <param name="key">The key to be used in the encryption cipher.</param>
        /// <returns>The encrypted string.</returns>
        public string Encrypt(SecureString input, string key)
        {
            //Salt and IV are randomly generated each time, but are prepended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting

            byte[] saltBytes = Generate256BitsOfRandomEntropy();
            byte[] ivBytes = Generate256BitsOfRandomEntropy();

            return Encrypt(input, key, saltBytes, ivBytes);
        }

        /// <summary>
        /// Encrypts a given <see cref="SecureString"/> input using a supplied key, salt and IV.
        /// </summary>
        /// <param name="input">The input to be encrypted.</param>
        /// <param name="key">The key to be used in the encryption cipher.</param>
        /// <param name="salt">The salt to be used in the encryption cipher.</param>
        /// <param name="iv">The Initialisation Vector to be used in the encryption cipher.</param>
        /// <returns>The encrypted string.</returns>
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
                            rijndael.CreateEncryptor(keyBytes, iv),
                            CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            //Create the final bytes as a conatenation of the random salt, IV and Cipher bytes
                            var cipherTextBytes = salt;
                            cipherTextBytes = cipherTextBytes.Concat(iv).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the salt for an encrypted <see cref="string"/>
        /// </summary>
        /// <param name="encryptedData">The encrypyted string.</param>
        /// <returns>The salt that was used to encrypt the string.</returns>
        public byte [] GetSalt(string encryptedData)
        {
            var cipherTextWithSaltAndIvBytes = Convert.FromBase64String(encryptedData);
            //get the salt by extracting the first 32 bytes
            var saltBytes = cipherTextWithSaltAndIvBytes.Take(KeySize / 8).ToArray();
            return saltBytes;
        }

        /// <summary>
        /// Retrieves the IV for an encrypted <see cref="string"/>
        /// </summary>
        /// <param name="encryptedData">The encrypyted string.</param>
        /// <returns>The IV that was used to encrypt the string.</returns>
        public byte[] GetIV(string encryptedData)
        {
            var cipherTextWithSaltAndIvBytes = Convert.FromBase64String(encryptedData);
            //get the iv by extracting the next 32 bytes
            var ivBytes = cipherTextWithSaltAndIvBytes.Skip(KeySize / 8).Take(KeySize / 8).ToArray();
            return ivBytes;
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
