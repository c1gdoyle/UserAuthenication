using System.Security;
using Demo.Presentation.Base;
using Prism.Commands;

namespace LogDemoApplication.ViewControllers
{
    public class LoginDemoViewController : ViewControllerBase
    {
        private string _userName;
        private SecureString _password;

        public LoginDemoViewController()
        {
            IntialiseCommands();
        }

        private void IntialiseCommands()
        {
            LoginCommand = new DelegateCommand(() => OnLoginCommand(), () => IsLoginEnabled);
            ExitCommand = new DelegateCommand(() => NotifiyWindowShouldClose(null));
        }

        /// <summary>
        /// Gets or sets the UserName for this ViewController.
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyPropertyChanged("UserName");
                LoginCommand.RaiseCanExecuteChanged();
                //NotifyPropertyChanged("IsLoginEnabled");
            }
        }

        public SecureString Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoginEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(UserName) &&
                    Password != null &&
                    Password.Length > 0;
            }
        }

        public DelegateCommand LoginCommand
        {
            get;
            private set;
        }

        public DelegateCommand ExitCommand
        {
            get;
            private set;
        }


        private void OnLoginCommand()
        {
        }
    }
}
