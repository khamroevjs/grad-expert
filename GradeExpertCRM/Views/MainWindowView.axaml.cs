using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GradeExpertCRM.Views
{
    public class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}