using System;
using System.Data;
using System.Data.Common;
using Demo.DataAccess.Base;
using Demo.DataAccess.Utilities;
using Demo.Security.Ciphers;
using Demo.Security.Extensions;
using LogDemoApplication.Authenication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Demo.LogDemoApplication.Test.Authenication
{
    [TestClass]
    public class LoginAuthenticationServiceTests
    {
        private const string UserId = "conor";
        private const string UnRegisteredUser = "Lucy";
        private const string CorrectPassword = "password1";
        private const string InCorrectPassword = "password2";
        private const string HashedPassword = "bHoFtpmknfxfB8s8qvlT7WvHfim5mCrmbDfBV9lZnL5I/bSeXP9laC7Qm5EDEAxdfFNTUdcttdEVSvkBebDFUQTmOGy2EJ+VYuFOKgTRx/0+JOPEekg/3O8LoiVRTgh7";
        private const string EncryptionKey = "MyEncryptionKey";

        [TestMethod]
        public void LoginAuthenticationServiceInitialises()
        {
            var service = new LoginAuthenticationService(MockDataAccess().Object, new RijnadelEncryptionCipher(), EncryptionKey);

            Assert.IsNotNull(service);
        }

        //[TestMethod]
        public void Login()
        {
            var cipher = new RijnadelEncryptionCipher();

            string hashedPassword = cipher.Encrypt(CorrectPassword.ToSecureString(), EncryptionKey);

            byte[] salt = cipher.GetSalt(hashedPassword);
            byte[] iv = cipher.GetIV(hashedPassword);

            string result = cipher.Encrypt(CorrectPassword.ToSecureString(), EncryptionKey, salt, iv);
        }

        [TestMethod]
        public void LoginAuthenicationServiceReturnsSuccessfulResultIfPasswordsMatch()
        {
            var service = new LoginAuthenticationService(MockDataAccess().Object, new RijnadelEncryptionCipher(), EncryptionKey);

            AuthenicationResult result = service.Authenicate(UserId, CorrectPassword.ToSecureString());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Login Authenication successful.", result.Message);
        }

        [TestMethod]
        public void LoginAuthenicationServiceReturnFailedResultIfPasswordsDoNotMatch()
        {
            var service = new LoginAuthenticationService(MockDataAccess().Object, new RijnadelEncryptionCipher(), EncryptionKey);

            AuthenicationResult result = service.Authenicate(UserId, InCorrectPassword.ToSecureString());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Login Authenication Failed. Invalid UserName or Password.", result.Message);
        }

        [TestMethod]
        public void LoginAuthenicationServiceReturnsFailedResultIfUserIdIsNotInDataBase()
        {
            var service = new LoginAuthenticationService(MockDataAccessForUserNotInDatabase().Object, new RijnadelEncryptionCipher(), EncryptionKey);

            AuthenicationResult result = service.Authenicate(UnRegisteredUser, InCorrectPassword.ToSecureString());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Login Authenication Failed. Invalid UserName or Password.", result.Message);
        }

        [TestMethod]
        public void LoginAuthenicationServiceReturnsFailedResultIfDataAccessLayerFails()
        {
            var service = new LoginAuthenticationService(MockFailingDataAccess().Object, new RijnadelEncryptionCipher(), EncryptionKey);

            AuthenicationResult result = service.Authenicate(UserId, CorrectPassword.ToSecureString());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Login Authenication Failed. Unable to connect to database. Please contact support.", result.Message);
        }

        private Mock<IDataAccessLayerFacade> MockDataAccess()
        {
            var dataAccess =  new Mock<IDataAccessLayerFacade>();
            dataAccess.Setup(da => da.FillTable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DbParameter []>())).
                Returns(FakeResults);

            return dataAccess;
        }

        private Mock<IDataAccessLayerFacade> MockDataAccessForUserNotInDatabase()
        {
            var dataAccess = new Mock<IDataAccessLayerFacade>();
            dataAccess.Setup(da => da.FillTable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DbParameter[]>())).
                Returns(null as DataTable);

            return dataAccess;
        }

        private Mock<IDataAccessLayerFacade> MockFailingDataAccess()
        {
            var dataAccess = new Mock<IDataAccessLayerFacade>();
            dataAccess.Setup(da => da.FillTable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DbParameter[]>())).
                Throws(new DataAccessLayerException("Failed to connection to SQL database"));

            return dataAccess;
        }

        private DataTable FakeResults()
        {
            DataTable table = new DataTable { TableName = "Authentication" };
            table.Columns.Add(new DataColumn("password", typeof(string)));

            var row = table.NewRow();
            row["password"] = HashedPassword;

            table.Rows.Add(row);
            return table;
        }
    }
}
