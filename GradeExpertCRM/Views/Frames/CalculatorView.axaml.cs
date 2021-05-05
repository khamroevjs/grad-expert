using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GradeExpertCRM.Views.Frames
{
    public class CalculatorView : UserControl
    {
        public CalculatorView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
