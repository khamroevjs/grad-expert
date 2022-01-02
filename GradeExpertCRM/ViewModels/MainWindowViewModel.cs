using System;
using GradeExpertCRM.Models.Data.Repositories;
using ReactiveUI;
using Splat;

namespace GradeExpertCRM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IBaseWindow
    {
        private string _language;

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

        /// <summary>
        /// Reference to the changing content of the app window.
        /// </summary>
        private ViewModelBase _content;

        public ViewModelBase Content
        {
            get => _content;
            set
            {
                value.BaseWindow = this;
                this.RaiseAndSetIfChanged(ref _content, value);
            }
        }

        /// <summary>
        /// The constructor initializes Content properties.
        /// </summary>
        public MainWindowViewModel()
        {
            var settingsRepository = Locator.Current.GetService<ISettingsRepository>();
            Language = settingsRepository.FirstOrDefault()?.Language ?? "Russian";
            Content = new SignInViewModel(this);
            //Content = new MainViewModel(true);
        }
    }
}