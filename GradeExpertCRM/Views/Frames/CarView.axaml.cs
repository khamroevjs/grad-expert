using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GradeExpertCRM.Views.Frames
{
    public class CarView : UserControl
    {
        public CarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
