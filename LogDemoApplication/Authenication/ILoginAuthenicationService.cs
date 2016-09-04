using System.Security;

namespace LogDemoApplication.Authenication
{
    /// <summary>
    /// Defines the behaviour of a class responsible for authenicating a login to the 
    /// application.
    /// </summary>
    public interface ILoginAuthenicationService
    {
        /// <summary>
        /// Authenicates a login attempt using a given userId and password.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="AuthenicationResult"/> detailing whether or not the login passed authenication.</returns>
        AuthenicationResult Authenicate(string userId, SecureString password);
    }
}
