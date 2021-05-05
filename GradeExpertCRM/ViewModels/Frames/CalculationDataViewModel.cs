using System.Collections.ObjectModel;
using ReactiveUI;
using Avalonia.Media.Imaging;
using GradeExpertCRM.Models;
using GradeExpertCRM.Models.Data.Repositories;
using Splat;
using System.Reactive;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GradeExpertCRM.ViewModels.Frames
{
    class CalculationDataViewModel : ViewModelBase
    {
        private readonly string carImagesPath
            = Path.GetFullPath(@"..\..\..\Resources\Cars\2\");

        private int LeftIndex = 0;

        private string[] _carImageNames =
        {
            "10-1", "09-1", "11-1", "14-1", "07-1",
            "05-1", "01-1", "04-1", "12-1", "08-1",
            "06-1", "02-1", "03-1", "13-1"
        };

        private Bitmap[] _carImages;

        private Bitmap[] _carImagesInInterface;

        private string[] _carImagesDescriprions = {"", "", "", ""};

        public Bitmap CarImage0
        {
            get => _carImagesInInterface[0];
            set => this.RaiseAndSetIfChanged(ref _carImagesInInterface[0], value);
        }

        public Bitmap CarImage1
        {
            get => _carImagesInInterface[1];
            set => this.RaiseAndSetIfChanged(ref _carImagesInInterface[1], value);
        }

        public Bitmap CarImage2
        {
            get => _carImagesInInterface[2];
            set => this.RaiseAndSetIfChanged(ref _carImagesInInterface[2], value);
        }

        public Bitmap CarImage3
        {
            get => _carImagesInInterface[3];
            set => this.RaiseAndSetIfChanged(ref _carImagesInInterface[3], value);
        }

        public string CarImageDescription0
        {
            get => _carImagesDescriprions[0];
            set => this.RaiseAndSetIfChanged(ref _carImagesDescriprions[0], value);
        }

        public string CarImageDescription1
        {
            get => _carImagesDescriprions[1];
            set => this.RaiseAndSetIfChanged(ref _carImagesDescriprions[1], value);
        }

        public string CarImageDescription2
        {
            get => _carImagesDescriprions[2];
            set => this.RaiseAndSetIfChanged(ref _carImagesDescriprions[2], value);
        }

        public string CarImageDescription3
        {
            get => _carImagesDescriprions[3];
            set => this.RaiseAndSetIfChanged(ref _carImagesDescriprions[3], value);
        }

        #region OverallPrices

        private double _dentRepairPrice;
        public double DentRepairPrice
        {
            get => _dentRepairPrice;
            set => this.RaiseAndSetIfChanged(ref _dentRepairPrice, value);
        }

        private double _dismantlingPrice;
        public double DismantlingPrice
        {
            get => _dismantlingPrice;
            set => this.RaiseAndSetIfChanged(ref _dismantlingPrice, value);
        }

        private double _sparePartsPrice;
        public double SparePartsPrice
        {
            get => _sparePartsPrice;
            set => this.RaiseAndSetIfChanged(ref _sparePartsPrice, value);
        }

        private double _paintingPrice;
        public double PaintingPrice
        {
            get => _paintingPrice;
            set => this.RaiseAndSetIfChanged(ref _paintingPrice, value);
        }

        private double _additionalWorksPrice;
        public double AdditionalWorksPrice
        {
            get => _additionalWorksPrice;
            set => this.RaiseAndSetIfChanged(ref _additionalWorksPrice, value);
        }

        public double _totalPrice;
        public double TotalPrice
        {
            get => _totalPrice;
            set => this.RaiseAndSetIfChanged(ref _totalPrice, value);
        }
        #endregion

        public bool IsButtonEnabled => carRepository_.SelectedCarId > 0 && settings_ != null;
        public ReactiveCommand<Calculation, Unit> DeleteCommand { get; }
        public ReactiveCommand<Calculation, Unit> ChangeCommand { get; }

        public OverallCalculation OverallCalculation { get; }

        private ObservableCollection<Calculation> _calculations;
        public ObservableCollection<Calculation> Calculations 
        {
            get => _calculations;
            private set
            {
                foreach (var calculation in value)
                {
                    switch (calculation.TypeOfRepair)
                    {
                        case TypeOfRepair.WithoutPainting:
                            calculation.TypeOfRepairString = Localization.WithoutPainting;
                            break;
                        case TypeOfRepair.UnderPainting:
                            calculation.TypeOfRepairString = Localization.UnderPainting;
                            break;
                        case TypeOfRepair.Replacement:
                            calculation.TypeOfRepairString = Localization.Replacement;
                            break;
                    }
                }
                _calculations = value;
            }
        }

        private readonly IOverallCalculationRepository overallCalculationRepository_;
        private readonly ICalculationRepository calculationRepository_;
        private readonly ICarRepository carRepository_;
        private readonly Settings settings_;

        public CalculationDataViewModel(IBaseWindow baseWindow)
        {
            _carImages = new Bitmap[]
            {
                new Bitmap(carImagesPath + @"\10-1.png"),
                new Bitmap(carImagesPath + @"\09-1.png"),
                new Bitmap(carImagesPath + @"\11-1.png"),
                new Bitmap(carImagesPath + @"\14-1.png"),
                new Bitmap(carImagesPath + @"\07-1.png"),
                new Bitmap(carImagesPath + @"\05-1.png"),
                new Bitmap(carImagesPath + @"\01-1.png"),
                new Bitmap(carImagesPath + @"\04-1.png"),
                new Bitmap(carImagesPath + @"\12-1.png"),
                new Bitmap(carImagesPath + @"\08-1.png"),
                new Bitmap(carImagesPath + @"\06-1.png"),
                new Bitmap(carImagesPath + @"\02-1.png"),
                new Bitmap(carImagesPath + @"\03-1.png"),
                new Bitmap(carImagesPath + @"\13-1.png"),
            };

            _carImagesInInterface = new Bitmap[]
            {
                new Bitmap(carImagesPath + @"\10-1.png"),
                new Bitmap(carImagesPath + @"\09-1.png"),
                new Bitmap(carImagesPath + @"\11-1.png"),
                new Bitmap(carImagesPath + @"\14-1.png"),
            };

            BaseWindow = baseWindow;
            UpdateImagesAndDescriptions();

            DeleteCommand = ReactiveCommand.CreateFromTask<Calculation>(Delete);
            ChangeCommand = ReactiveCommand.Create<Calculation>(Change);

            carRepository_ = Locator.Current.GetService<ICarRepository>();
            settings_ = Locator.Current.GetService<ISettingsRepository>().FirstOrDefault();
            var selectedCarId = carRepository_.SelectedCarId;
            if (selectedCarId == 0 || settings_ == null)
                return;

            overallCalculationRepository_ = Locator.Current.GetService<IOverallCalculationRepository>();
            calculationRepository_ = Locator.Current.GetService<ICalculationRepository>();

            var calculations = calculationRepository_.GetFullCalculationsByCarId(selectedCarId);
            Calculations = new ObservableCollection<Calculation>(calculations);
            if (Calculations == null)
                return;

            var overallCalculation = overallCalculationRepository_.GetByCarId(selectedCarId);
            OverallCalculation = overallCalculation ?? new OverallCalculation();
            UpdateOverallCalculation();
            UpdatePrices();
        }

        #region CheckBoxes

        public bool ToolPreparingCheckBox
        {
            get 
            {
                if (OverallCalculation == null)
                    return false;
                return OverallCalculation.IsPreparingToolApplied;
            } 
            set
            {
                OverallCalculation.IsPreparingToolApplied = value;
                UpdateOverallCalculation();
                UpdatePrices();
            }
        }

        public bool AntiCorrosionTreatmentCheckBox
        {
            get
            {
                if (OverallCalculation == null)
                    return false;
                return OverallCalculation.IsAntiCorrosionApplied;
            }
            set
            {
                OverallCalculation.IsAntiCorrosionApplied = value;
                UpdateOverallCalculation();
                UpdatePrices();
            }
        }

        public bool BoxFinishingTreatmentCheckBox
        {
            get
            {
                if (OverallCalculation == null)
                    return false;
                return OverallCalculation.IsFinalProcessingApplied;
            }
            set
            {
                OverallCalculation.IsFinalProcessingApplied = value;
                UpdateOverallCalculation();
                UpdatePrices();
            }
        }
        
        public bool TechnicalWashCheckBox
        {
            get
            {
                if (OverallCalculation == null)
                    return false;
                return OverallCalculation.IsTechnicalWashApplied;
            }
            set
            {
                OverallCalculation.IsTechnicalWashApplied = value;
                UpdateOverallCalculation();
                UpdatePrices();
            }
        }
        
        public bool SalonCleaningCheckBox
        {
            get
            {
                if (OverallCalculation == null)
                    return false;
                return OverallCalculation.IsSalonCleaningApplied;
            }
            set
            {
                OverallCalculation.IsSalonCleaningApplied = value;
                UpdateOverallCalculation();
                UpdatePrices();
            }
        }
        

        #endregion
        
        private void UpdatePrices()
        {
            double currentDentPrice = 0;
            double currentPainting = 0;
            double currentDismantlingPrice = 0;
            double currentSparePartsPrice = 0;
            foreach (var calculation in Calculations)
            {
                currentDentPrice += calculation.DentPrice;
                currentPainting += calculation.PriceOfPainting;

                currentDismantlingPrice += calculation.DismantlingWorks.Where(work=>work.IsApplied).Sum(work => work.Price);

                foreach (var sparePart in calculation.SpareParts)
                    currentSparePartsPrice += sparePart.Price;
            }

            DentRepairPrice = currentDentPrice;
            PaintingPrice = currentPainting;
            DismantlingPrice = currentDismantlingPrice;
            SparePartsPrice = currentSparePartsPrice;
            AdditionalWorksPrice = OverallCalculation.AntiCorrosionPrice +
                                    OverallCalculation.PreparingToolPrice +
                                    OverallCalculation.FinalProcessingPrice +
                                    OverallCalculation.TechnicalWashPrice + 
                                    OverallCalculation.SalonCleaningPrice;

            TotalPrice = DentRepairPrice + DismantlingPrice + SparePartsPrice + PaintingPrice + AdditionalWorksPrice;
        }

        private void UpdateOverallCalculation()
        {
            OverallCalculation.TaxPercent = settings_.TaxPercent;
            OverallCalculation.CarId = carRepository_.SelectedCarId;

            if (OverallCalculation.IsPreparingToolApplied)
                OverallCalculation.PreparingToolPrice = settings_.PreparingTool * Calculations.Count;
            else
                OverallCalculation.PreparingToolPrice = 0;
            
            if (OverallCalculation.IsAntiCorrosionApplied)
                OverallCalculation.AntiCorrosionPrice = settings_.AntiCorrosion * Calculations.Count;
            else
                OverallCalculation.AntiCorrosionPrice = 0;
            
            if (OverallCalculation.IsFinalProcessingApplied)
            {
                var price = settings_.FinalProcessing * Calculations.Count;
                var maxPrice = settings_.FinalProcessingMax;
                OverallCalculation.FinalProcessingPrice = price > maxPrice ? maxPrice : price;
            }
            else
                OverallCalculation.FinalProcessingPrice = 0;

            if (OverallCalculation.IsTechnicalWashApplied)
                OverallCalculation.TechnicalWashPrice = settings_.TechnicalWash;
            else
                OverallCalculation.TechnicalWashPrice = 0;

            if (OverallCalculation.IsSalonCleaningApplied)
                OverallCalculation.SalonCleaningPrice = settings_.SalonCleaning;
            else
                OverallCalculation.SalonCleaningPrice = 0;

            overallCalculationRepository_.Update(OverallCalculation);
        }

        public void Change(Calculation calculation)
        {
            BaseWindow.Content = new CalculatorViewModel(BaseWindow, calculation);
        }

        public async Task Delete(Calculation calculation)
        {
            Calculations.Remove(calculation);
            await calculationRepository_.RemoveAsync(calculation);
        }

        public void ScrollLeft()
        {
            LeftIndex = (LeftIndex + _carImages.Length - 1) % _carImages.Length;
            UpdateImagesAndDescriptions();
        }

        public void ScrollRight()
        {
            LeftIndex = (LeftIndex + 1) % _carImages.Length;
            UpdateImagesAndDescriptions();
        }

        public void SelectDetail(string imageNumber)
        {
            string carImageName = imageNumber switch
            {
                "image0" => _carImageNames[LeftIndex],
                "image1" => _carImageNames[(LeftIndex + 1) % _carImages.Length],
                "image2" => _carImageNames[(LeftIndex + 2) % _carImages.Length],
                "image3" => _carImageNames[(LeftIndex + 3) % _carImages.Length],
                _ => ""
            };

            BaseWindow.Content = new CalculatorViewModel(BaseWindow, carImageName);
        }

        private void UpdateImagesAndDescriptions()
        {
            CarImage0 = _carImages[LeftIndex];
            CarImage1 = _carImages[(LeftIndex + 1) % _carImages.Length];
            CarImage2 = _carImages[(LeftIndex + 2) % _carImages.Length];
            CarImage3 = _carImages[(LeftIndex + 3) % _carImages.Length];
            CarImageDescription0 = GetCarImageDescription(LeftIndex);
            CarImageDescription1 = GetCarImageDescription((LeftIndex + 1) % _carImages.Length);
            CarImageDescription2 = GetCarImageDescription((LeftIndex + 2) % _carImages.Length);
            CarImageDescription3 = GetCarImageDescription((LeftIndex + 3) % _carImages.Length);
        }

        private string GetCarImageDescription(int index)
            => _carImageNames[index] switch
            {
                "01-1" => Localization.FrontLeftDoor,
                "02-1" => Localization.FrontRightDoor,
                "03-1" => Localization.RearRightDoor,
                "04-1" => Localization.RearLeftDoor,
                "05-1" => Localization.FrontLeftFender,
                "06-1" => Localization.FrontRightFender,
                "07-1" => Localization.LeftRack,
                "08-1" => Localization.RightRack,
                "09-1" => Localization.Roof,
                "10-1" => Localization.Hood,
                "11-1" => Localization.Trunk,
                "12-1" => Localization.RearLeftFender,
                "13-1" => Localization.RearRightFender,
                "14-1" => Localization.RearBumper,
                _ => "Error"
            };
    }
}