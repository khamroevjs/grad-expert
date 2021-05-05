using System.Collections.ObjectModel;
using GradeExpertCRM.Models;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using GradeExpertCRM.Models.Data.Repositories;
using Splat;
using System.Linq;

namespace GradeExpertCRM.ViewModels.Frames
{
    class ClientViewModel : ViewModelBase
    {
        private string _searchString;

        private ObservableCollection<Client> _allClients;

        private ObservableCollection<Client> _clients;

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref _clients, value);
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchString, value);
                if (_allClients != null)
                {
                    var clients = _allClients.Where(client => client.Name.Contains(_searchString));
                    Clients = new ObservableCollection<Client>(clients);
                }
            }
        }

        private async Task OpenAddingClientView() => BaseWindow.Content = new AddingClientViewModel(BaseWindow);

        public ReactiveCommand<Unit, Unit> GoAddingClientView { get; }
        
        public ReactiveCommand<Client, Unit> ChangeCommand { get; }
        public ReactiveCommand<Client, Unit> DeleteCommand { get; }

        private IClientRepository clientRepository_;

        public ClientViewModel(IBaseWindow baseWindow)
        {
            BaseWindow = baseWindow;
            GoAddingClientView = ReactiveCommand.CreateFromTask(OpenAddingClientView);
            ChangeCommand = ReactiveCommand.CreateFromTask<Client>(Change);
            DeleteCommand = ReactiveCommand.CreateFromTask<Client>(Delete);
            SearchString = "";
            clientRepository_ = Locator.Current.GetService<IClientRepository>();
            var clients = clientRepository_.All();
            _allClients = new ObservableCollection<Client>(clients);
            Clients = _allClients;
        }

        private async Task Change(Client client)
        {
            BaseWindow.Content = new AddingClientViewModel(BaseWindow, client);
        }
        
        private async Task Delete(Client client)
        {
            Clients.Remove(client);
            await clientRepository_.RemoveAsync(client);
        }
    }
}