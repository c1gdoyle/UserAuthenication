using System;
using System.Configuration;
using System.Data;
using System.Security;
using Demo.DataAccess.Base;
using Demo.DataAccess.Utilities;
using Demo.Security.Base;
using Demo.Security.Ciphers;

namespace LogDemoApplication.Authenication
{
    /// <summary>
    /// An implemenation of <see cref="ILoginAuthenicationService"/> that checks passwords entered into the Login form
    /// against user information stored in a configured SQL data-base to authenicate.
    /// </summary>
    public class LoginAuthenticationService : ILoginAuthenicationService
    {
        private readonly IDataAccessLayerFacade _dataAccess;
        private readonly IEncryptionCipher _encryptionCipher;
        private readonly string _encryptionKey;

        public LoginAuthenticationService()
        {
            _encryptionCipher = new RijnadelEncryptionCipher();
            _dataAccess = new DataAccessLayerFacade(() => new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["HumanResources"].ConnectionString));
            _encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
        }

        /// <summary>
        /// Initialises a new instance of <see cref="LoginAuthenticationService"/> with a specified dataAccess facade, cipher
        /// and key.
        /// </summary>
        /// <param name="dataAccess">The data access facade.</param>
        /// <param name="encryptionCipher">The encryption cipher.</param>
        /// <param name="key">The encryption key.</param>
        public LoginAuthenticationService(IDataAccessLayerFacade dataAccess, IEncryptionCipher encryptionCipher, string key)
        {
            _dataAccess = dataAccess;
            _encryptionCipher = encryptionCipher;
            _encryptionKey = key;
        }

        public AuthenicationResult Authenicate(string userId, SecureString password)
        {
            DataTable table = null;
            string dataBasePassword;
            try
            {
                table = GetData(userId);
            }
            catch(Exception)
            {
                return new AuthenicationResult { IsSuccessful = false, Message = "Login Authenication Failed. Unable to connect to database. Please contact support." };
            }

            if (!TryGetPassWord(table, out dataBasePassword))
            {
                return new AuthenicationResult { IsSuccessful = false, Message = "Login Authenication Failed. Invalid UserName or Password." };
            }

            return ComparePasswords(password, dataBasePassword);
        }

        private DataTable GetData(string userId)
        {
            DataTable table = 
                _dataAccess.FillTable(
                    "SELECT password FROM dbo.Users WHERE userName = '{0}'", 
                    "AuthenicationResult", 
                    new object[] { userId });

            return table;
        }

        private bool TryGetPassWord(DataTable table, out string dataBasePassword)
        {
            dataBasePassword = null;
            if(table != null && table.Rows != null && table.Rows.Count == 1)
            {
                dataBasePassword = (string)table.Rows[0]["password"];
                return true;
            }
            return false;
        }

        private AuthenicationResult ComparePasswords(SecureString password, string dataBasePassword)
        {
            byte[] salt = _encryptionCipher.GetSalt(dataBasePassword);
            byte[] iv = _encryptionCipher.GetIV(dataBasePassword);

            string hashedPassword = _encryptionCipher.Encrypt(password, _encryptionKey, salt, iv);

            if (dataBasePassword == hashedPassword)
            {
                return new AuthenicationResult { IsSuccessful = true, Message = "Login Authenication successful." };
            }
            else
            {
                return new AuthenicationResult { IsSuccessful = false, Message = "Login Authenication Failed. Invalid UserName or Password." };
            }
        }
    }
}
