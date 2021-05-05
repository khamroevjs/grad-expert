using System.Threading.Tasks;
using ReactiveUI;
using GradeExpertCRM.ViewModels.Frames;
using System.Reactive;
using GradeExpertCRM.Models;
using GradeExpertCRM.Models.Data.Repositories;
using Splat;

namespace GradeExpertCRM.ViewModels
{
    class MainViewModel : ViewModelBase, IBaseWindow
    {
        private string _language;

        private ViewModelBase _content;

        private ILanguageProvider _localization;

        private ILanguageProvider[] LanguageProvider = new ILanguageProvider[]
        {
            new RussianLanguageProvider(),
            new GermanLanguageProvider(),
            new EnglishLanguageProvider()
        };

        public string Language
        {
            get => _language;
            set
            {
                this.RaiseAndSetIfChanged(ref _language, value);
                Localization = _language switch
                {
                    "Russian" => LanguageProvider[0],
                    "German" => LanguageProvider[1],
                    _ => LanguageProvider[2]
                };
            }
        }

        public new ILanguageProvider Localization
        {
            get => _localization;
            set => this.RaiseAndSetIfChanged(ref _localization, value);
        }

        public ViewModelBase Content
        {
            get => _content;
            set
            {
                _content = this;
                this.RaiseAndSetIfChanged(ref _content, value);
            }
        }

        private async Task OpenClientWindow() => Content = new ClientViewModel(this);

        private async Task OpenCarView() => Content = new CarViewModel(this);

        private async Task OpenCalculatorView() => Content = new CalculationDataViewModel(this);

        private async Task OpenDocumentsView() => Content = new DocumentsViewModel(this);

        private async Task OpenMailView() => Content = new MailViewModel(this);

        private async Task OpenSettingsView() => Content = new SettingsViewModel(this);


        public ReactiveCommand<Unit, Unit> GoClientWindow { get; }

        public ReactiveCommand<Unit, Unit> GoCarView { get; }

        public ReactiveCommand<Unit, Unit> GoCalculatorView { get; }

        public ReactiveCommand<Unit, Unit> GoDocumentsView { get; }

        public ReactiveCommand<Unit, Unit> GoMailView { get; }

        public ReactiveCommand<Unit, Unit> GoSettingsView { get; }

        public MainViewModel(bool isAdmin)
        {
            IsAdmin = isAdmin;

            GoClientWindow = ReactiveCommand.CreateFromTask(OpenClientWindow);
            GoCarView = ReactiveCommand.CreateFromTask(OpenCarView);
            GoCalculatorView = ReactiveCommand.CreateFromTask(OpenCalculatorView);
            GoDocumentsView = ReactiveCommand.CreateFromTask(OpenDocumentsView);
            GoMailView = ReactiveCommand.CreateFromTask(OpenMailView);
            GoSettingsView = ReactiveCommand.CreateFromTask(OpenSettingsView);

            var settingsRepository = Locator.Current.GetService<ISettingsRepository>();
            Language = settingsRepository.FirstOrDefault()?.Language ?? "Russian";

            Content = new ClientViewModel(this);
        }
    }
}