using System;
using System.Data;
using System.Data.Common;
using Demo.DataAccess.Base;
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
        private const string HashedPassword = "K4o4kfaFKGAptQlQyYJeMLcDaNVrKpCw6xJCEtNVxesrijiR9oUoYCm1CVDJgl4wtwNo1WsqkLDrEkIS01XF6xfVTLy+5gSEETkO4WV9yP9Ij+DWaZZTYiFYFWUCMsJ9";
        private const string EncryptionKey = "MyEncryptionKey";

        [TestMethod]
        public void LoginAuthenticationServiceInitialises()
        {
            var service = new LoginAuthenticationService(MockDataAccess().Object, new RijnadelEncryptionCipher(), EncryptionKey);

            Assert.IsNotNull(service);
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
                Throws(new Exception("Failed to connection to SQL database"));

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
