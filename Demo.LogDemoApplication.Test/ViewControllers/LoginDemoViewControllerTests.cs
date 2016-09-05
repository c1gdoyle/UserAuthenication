using Demo.Presentation.Dialog;
using Demo.Security.Extensions;
using LogDemoApplication.Authenication;
using LogDemoApplication.ViewControllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Demo.LogDemoApplication.Test.ViewControllers
{
    [TestClass]
    public class LoginDemoViewControllerTests
    {
        [TestMethod]
        public void ByDefaultLoginCommandIsDisabled()
        {
            var controller = new LoginDemoViewController(
                new Mock<ILoginAuthenicationService>().Object, 
                new Mock<IDialogService>().Object);

            Assert.IsNotNull(controller);
            Assert.IsFalse(controller.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void LoginCommandRemainsDisabledIfOnlyUserNameIsSet()
        {
            var controller = new LoginDemoViewController(
                new Mock<ILoginAuthenicationService>().Object,
                new Mock<IDialogService>().Object);

            controller.UserName = "conor";
            
            Assert.IsFalse(controller.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void LoginCommandRemainsDisabledIfOnlyPasswordIsSet()
        {
            var controller = new LoginDemoViewController(
                new Mock<ILoginAuthenicationService>().Object, 
                new Mock<IDialogService>().Object);

            controller.Password = "password1".ToSecureString();

            Assert.IsFalse(controller.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void LoginCommandIsEnabledIfBothUserNameAndPasswordAreSet()
        {
            var controller = new LoginDemoViewController(
                new Mock<ILoginAuthenicationService>().Object,
                new Mock<IDialogService>().Object);

            controller.UserName = "conor";
            controller.Password = "password1".ToSecureString();

            Assert.IsTrue(controller.LoginCommand.CanExecute());
        }
    }
}
