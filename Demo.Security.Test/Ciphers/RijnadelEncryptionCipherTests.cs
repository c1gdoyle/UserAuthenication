using Demo.Security.Ciphers;
using Demo.Security.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Demo.Security.Test.Ciphers
{
    [TestClass]
    public class RijnadelEncryptionCipherTests
    {
        private const string UnEncryptedText = "Voynich";
        private const string EncryptionKey = "Vigenere";

        [TestMethod]
        public void CipherEncryptsSecureString()
        {
            var cipher = new RijnadelEncryptionCipher();
            string encryptedResult = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);

            Assert.AreNotEqual(UnEncryptedText, encryptedResult);
        }

        [TestMethod]
        public void CipherWillEncryptSameInputToDifferenctEncrypedResult()
        {
            var cipher = new RijnadelEncryptionCipher();
            string encryptedResult1 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);
            string encryptedResult2 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);

            Assert.AreNotEqual(encryptedResult2, encryptedResult1);
        }

        [TestMethod]
        public void CipherUsesDifferentSaltForEachEncryption()
        {
            var cipher = new RijnadelEncryptionCipher();
            string encryptedResult1 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);
            string encryptedResult2 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);

            byte[] salt1 = cipher.GetSalt(encryptedResult1);
            byte[] salt2 = cipher.GetSalt(encryptedResult2);

            Assert.AreNotEqual(salt1, salt2);
        }

        [TestMethod]
        public void CipherUsesDifferentIVForEachEncryption()
        {
            var cipher = new RijnadelEncryptionCipher();
            string encryptedResult1 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);
            string encryptedResult2 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);

            byte[] iv1 = cipher.GetIV(encryptedResult1);
            byte[] iv2 = cipher.GetIV(encryptedResult2);

            Assert.AreNotEqual(iv1, iv2);
        }

        [TestMethod]
        public void CipherWillEncryptSameInputToSameResultGivenSaltAndIV()
        {
            var cipher = new RijnadelEncryptionCipher();
            string encryptedResult1 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);
            byte[] salt = cipher.GetSalt(encryptedResult1);
            byte[] iv = cipher.GetIV(encryptedResult1);

            string encryptedResult2 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey, salt, iv);

            Assert.AreEqual(encryptedResult1, encryptedResult2);
        }
    }
}
