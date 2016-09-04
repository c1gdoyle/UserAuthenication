using System;
using System.Security;

namespace Demo.Security.Extensions
{
    /// <summary>
    /// Provides extension methods for converting between secure and insecure strings.
    /// </summary>
    public static class SecureStringExtensions
    {
        /// <summary>
        /// Converts this <see cref="SecureString"/> into an insecure <see cref="string"/>.
        /// </summary>
        /// <param name="secure">The secure string.</param>
        /// <returns>The insecure string.</returns>
        public static string ToInSecureString(this SecureString secure)
        {
            string result = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secure);
            try
            {
                result = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return result;
        }

        /// <summary>
        /// Converts this insecure <see cref="string"/> into a <see cref="SecureString"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The secure string.</returns>
        public static SecureString ToSecureString(this string s)
        {
            SecureString secure = new SecureString();

            foreach (char c in s)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }
    }
}
