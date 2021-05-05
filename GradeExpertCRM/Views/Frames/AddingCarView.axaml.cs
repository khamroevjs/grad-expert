using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GradeExpertCRM.Views.Frames
{
    public class AddingCarView : UserControl
    {
        public AddingCarView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
