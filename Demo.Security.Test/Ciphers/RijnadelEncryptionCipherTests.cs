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
        public void CipherWillEncryptSameToInputToDifferenctEncrypedResult()
        {
            var cipher = new RijnadelEncryptionCipher();
            string encryptedResult1 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);
            string encryptedResult2 = cipher.Encrypt(UnEncryptedText.ToSecureString(), EncryptionKey);

            Assert.AreNotEqual(encryptedResult2, encryptedResult1);
        }
    }
}
