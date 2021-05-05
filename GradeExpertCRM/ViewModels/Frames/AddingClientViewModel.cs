using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using GradeExpertCRM.Models;
using GradeExpertCRM.Models.Data.Repositories;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using Splat;

namespace GradeExpertCRM.ViewModels.Frames
{
    class AddingClientViewModel : ViewModelBase
    {
        public Client Client { get; set; } = new Client();
        private IClientRepository clientRepository_;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        
        public int RoleOfClient
        {
            get => (int) Client.Role;
            set
            {
                Client.Role = (RoleEnum) value;
                this.RaisePropertyChanged(nameof(RoleOfClient));
            }
        }

        public AddingClientViewModel(IBaseWindow baseWindow)
        {
            BaseWindow = baseWindow;
            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            clientRepository_ = Locator.Current.GetService<IClientRepository>();
        }

        public AddingClientViewModel(IBaseWindow baseWindow, Client client) : this(baseWindow)
        {
            Client = client;
        }

        public async Task SaveAsync()
        {
            var validationContext = new ValidationContext(Client) {MemberName = nameof(Client)};
            var isValid = Validator.TryValidateObject(Client, validationContext, null);
            if (!isValid)
                return;

            await clientRepository_.UpdateAsync(Client);
            BaseWindow.Content = new ClientViewModel(BaseWindow);

            //try
            //{
            //    await repository_.AddAsync(Client);
            //    BaseWindow.Content = new ClientViewModel(BaseWindow);
            //}
            //catch
            //{
            //    await MessageBox.Avalonia.MessageBoxManager
            //        .GetMessageBoxStandardWindow(Localization.Error, Localization.IncorrectFillingInOfFields,
            //        ButtonEnum.Ok, Icon.Error, WindowStartupLocation.CenterScreen, Style.MacOs)
            //        .Show();
            //}
        }
    }
}