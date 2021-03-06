﻿using System.Security;

namespace Demo.Security.Base
{
    /// <summary>
    /// Defines the behaviour of a class responsible for encryption.
    /// </summary>
    public interface IEncryptionCipher
    {
        /// <summary>
        /// Encrypts a given <see cref="SecureString"/> input using a supplied key.
        /// </summary>
        /// <param name="input">The input to be encrypted.</param>
        /// <param name="key">The key to be used in the encryption cipher.</param>
        /// <returns>The encrypted string.</returns>
        string Encrypt(SecureString input, string key);

        /// <summary>
        /// Encrypts a given <see cref="SecureString"/> input using a supplied key, salt and IV.
        /// </summary>
        /// <param name="input">The input to be encrypted.</param>
        /// <param name="key">The key to be used in the encryption cipher.</param>
        /// <param name="salt">The salt to be used in the encryption cipher.</param>
        /// <param name="iv">The Initialisation Vector to be used in the encryption cipher.</param>
        /// <returns>The encrypted string.</returns>
        string Encrypt(SecureString input, string key, byte[] salt, byte[] iv);

        /// <summary>
        /// Retrieves the salt for an encrypted <see cref="string"/>
        /// </summary>
        /// <param name="encryptedText">The encrypyted string.</param>
        /// <returns>The salt that was used to encrypt the string.</returns>
        byte[] GetSalt(string encryptedText);

        /// <summary>
        /// Retrieves the IV for an encrypted <see cref="string"/>
        /// </summary>
        /// <param name="encryptedText">The encrypyted string.</param>
        /// <returns>The IV that was used to encrypt the string.</returns>
        byte[] GetIV(string encryptedText);
    }
}
