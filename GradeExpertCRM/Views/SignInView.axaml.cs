using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GradeExpertCRM.Views
{
    public class SignInView : UserControl
    {
        public SignInView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}