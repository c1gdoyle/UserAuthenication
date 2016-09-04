using System.Data;
using System.Security;
using Demo.DataAccess.Base;
using Demo.Security.Base;

namespace LogDemoApplication.Authenication
{
    public class LoginAuthenticationService : ILoginAuthenicationService
    {
        private readonly IDataAccessLayerFacade _dataAccess;
        private readonly IEncryptionCipher _encryptionCipher;

        public LoginAuthenticationService(IDataAccessLayerFacade dataAccess, IEncryptionCipher encryptionCipher)
        {
            _dataAccess = dataAccess;
            _encryptionCipher = encryptionCipher;
        }

        public AuthenicationResult Authenicate(string userId, SecureString password)
        {
            DataTable result = _dataAccess.FillTable(
                "SELECT * FROM dbo.Users WHERE userId = {0}", 
                "AuthenicationResult", 
                new object[] { userId });

            string encrypytedPassword = (string)result.Rows[0]["Password"];
                        
            return null;
        }
    }
}
