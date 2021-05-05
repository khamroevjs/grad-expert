using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GradeExpertCRM.ViewModels;
using GradeExpertCRM.Views;
using Microsoft.Extensions.Configuration;

namespace GradeExpertCRM
{
    public class App : Application
    {
        public static string ConnectionString { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            ConnectionString = configuration.GetConnectionString("DefaultConnection");
            _ = new AppBootstrapper();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindowView
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
