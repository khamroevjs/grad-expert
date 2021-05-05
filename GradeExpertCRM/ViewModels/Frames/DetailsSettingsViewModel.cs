using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using GradeExpertCRM.Models;
using GradeExpertCRM.Models.Data.Repositories;
using MessageBox.Avalonia.Views;
using Microsoft.EntityFrameworkCore.Internal;
using ReactiveUI;
using Splat;

namespace GradeExpertCRM.ViewModels.Frames
{
    class DetailsSettingsViewModel : ViewModelBase
    {
        public DetailsSettings DetailsSettings { get; set; }

        private Bitmap image_ = new Bitmap(@"..\..\..\Resources\plus.png");

        public Bitmap Image
        {
            get => image_;
            set => this.RaiseAndSetIfChanged(ref image_, value);
        }

        private readonly IDetailsSettingsRepository detailsRepository_;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ChooseImageCommand { get; }

        public DetailsSettingsViewModel(IBaseWindow baseWindow)
        {
            BaseWindow = baseWindow;
            detailsRepository_ = Locator.Current.GetService<IDetailsSettingsRepository>();
            DetailsSettings = detailsRepository_.FirstOrDefault() ?? new DetailsSettings();

            if (DetailsSettings?.Image != null)
            {
                using var ms = new MemoryStream(DetailsSettings.Image);
                Image = new Bitmap(ms);
            }

            SaveCommand = ReactiveCommand.CreateFromTask(Save);
            ChooseImageCommand = ReactiveCommand.CreateFromTask(ChooseImage);
        }

        public async Task Save()
        {
            var validationContext = new ValidationContext(DetailsSettings) { MemberName = nameof(DetailsSettings) };
            var isValid = Validator.TryValidateObject(DetailsSettings, validationContext, null);
            if (!isValid)
                return;

            await detailsRepository_.UpdateAsync(DetailsSettings);
        }

        public async Task ChooseImage()
        {
            OpenFileDialog dialog = new OpenFileDialog { AllowMultiple = false };
            dialog.Filters.Add(new FileDialogFilter { Name = "Image", Extensions = { "png", "jpg", "jpeg" } });

            string[] result = await dialog.ShowAsync(new Window());

            if (result.Length > 0)
            {
                Image = new Bitmap(result[0]);

                DetailsSettings.Image = await File.ReadAllBytesAsync(result[0]);
            }
        }
    }
}
