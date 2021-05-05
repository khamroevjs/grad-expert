using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Extensions.Configuration;

namespace GradeExpertCRM.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        #region UserData

        private string _login;

        public string Login
        {
            get => _login;
            set
            {
                _login = this.RaiseAndSetIfChanged(ref _login, value);
                CheckForEmptyForm();
            }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                _password = this.RaiseAndSetIfChanged(ref _password, value);
                CheckForEmptyForm();
            }
        }

        #endregion

        private bool _isEnabledSignIn = false;

        /// <summary>
        /// Implements the logic of the authorization completion button.
        /// </summary>
        public bool IsEnabledSignIn
        {
            get => _isEnabledSignIn;
            set
            {
                _isEnabledSignIn = this.RaiseAndSetIfChanged(ref _isEnabledSignIn, value);
            }
        }

        private bool _isFormEnabled = true;

        /// <summary>
        /// Implements verification of the correctness of filling out the authorization form.
        /// </summary>
        public bool IsFormEnabled
        {
            get => _isFormEnabled;
            set => this.RaiseAndSetIfChanged(ref _isFormEnabled, value);
        }

        private string _error;

        /// <summary>
        /// Implements the logic of the error message displayed in the UI.
        /// </summary>
        public string Error
        {
            get => _error;
            set => this.RaiseAndSetIfChanged(ref _error, value);
        }

        public ReactiveCommand<Unit, Unit> SignInCommand { get; }

        public SignInViewModel(IBaseWindow baseWindow)
        {
            BaseWindow = baseWindow;
            SignInCommand = ReactiveCommand.CreateFromTask(SignInAsync);
        }

        /// <summary>
        /// Implements access to the button to complete the authorization.
        /// </summary>
        private void CheckForEmptyForm()
            => IsEnabledSignIn = !(string.IsNullOrWhiteSpace(_login) 
                || string.IsNullOrWhiteSpace(_password));

        /// <summary>
        /// Implements user authorization.
        /// </summary>
        private async Task SignInAsync()
        {
            IsFormEnabled = false;
            try
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                if (Login == "Admin" && Password == configuration["AdminPassword"])
                {
                    BaseWindow.Content = new MainViewModel(true);
                }
                if (Login == "User" && Password == configuration["UserPassword"])
                {
                    BaseWindow.Content = new MainViewModel(true);
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                IsFormEnabled = true;
            }
        }
    }
}