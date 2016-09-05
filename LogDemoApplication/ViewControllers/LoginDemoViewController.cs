using System.Security;
using Demo.Presentation.Base;
using Demo.Presentation.Dialog;
using LogDemoApplication.Authenication;
using Prism.Commands;

namespace LogDemoApplication.ViewControllers
{
    public class LoginDemoViewController : ViewControllerBase
    {
        private readonly ILoginAuthenicationService _authenticationService;
        private readonly IDialogService _dialogService;
        private string _userName;
        private SecureString _password;

        /// <summary>
        /// Intialises a new default instance of <see cref="LoginDemoViewController"/>.
        /// </summary>
        public LoginDemoViewController()
            :this(new LoginAuthenticationService(), new DialogService())
        {
        }

        /// <summary>
        /// Intialises a instance of <see cref="LoginDemoViewController"/> with a specified authenication and dialog service.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public LoginDemoViewController(ILoginAuthenicationService authenticationService, IDialogService dialogService)
        {
            _authenticationService = authenticationService;
            _dialogService = dialogService;
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
            }
        }

        /// <summary>
        /// Gets or sets a reference to the password.
        /// </summary>
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
            var result = _authenticationService.Authenicate(UserName, Password);

            if(result != null)
            {
                _dialogService.ShowMessage(result.Message, result.IsSuccessful);
            }
        }
    }
}
