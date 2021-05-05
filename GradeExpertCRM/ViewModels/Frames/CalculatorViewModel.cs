using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using Avalonia.Media.Imaging;
using DynamicData;
using GradeExpertCRM.Models;
using GradeExpertCRM.Models.Data.Repositories;
using iText.Layout.Element;
using Newtonsoft.Json;
using Splat;

namespace GradeExpertCRM.ViewModels.Frames
{
    internal class CalculatorViewModel : ViewModelBase
    {
        private bool _isPriceEnabled = false;

        private const string ResourcesDirectory = @"..\..\..\Resources\";
        private string _carImageName = "01-1";

        private const string ImagePath = @"..\..\..\Resources\Cars\2\";

        private Bitmap _carImage = new Bitmap(@"..\..\..\Resources\Cars\2\01-1.png");

        public Bitmap CarImage
        {
            get => _carImage;
            set => this.RaiseAndSetIfChanged(ref _carImage, value);
        }

        private int dentDiameterIndex_;

        public int DentDiameterIndex
        {
            get => dentDiameterIndex_;
            set
            {
                dentDiameterIndex_ = value;
                Calculation.DentDiameter = value switch
                {
                    0 => 20,
                    1 => 30,
                    _ => 40
                };
            }
        }

        private double _nHours;

        public double NHours
        {
            get => _nHours;
            set => this.RaiseAndSetIfChanged(ref _nHours, value);
        }

        private double _removeDentPrice;

        public double RemoveDentPrice
        {
            get => _removeDentPrice;
            set => this.RaiseAndSetIfChanged(ref _removeDentPrice, value);
        }

        private double price_;

        public double Price
        {
            get => price_;
            set => this.RaiseAndSetIfChanged(ref price_, value);
        }

        private double dentPrice_;

        public double DentPriceView
        {
            get => dentPrice_;
            set => this.RaiseAndSetIfChanged(ref dentPrice_, value);
        }

        private double dismantlingPrice_;

        public double DismantlingPrice
        {
            get => dismantlingPrice_;
            set => this.RaiseAndSetIfChanged(ref dismantlingPrice_, value);
        }

        private double sparePartsPrice_;

        public double SparePartsPrice
        {
            get => sparePartsPrice_;
            set => this.RaiseAndSetIfChanged(ref sparePartsPrice_, value);
        }

        private bool priceOfPaintingIsEnabled;

        public bool PriceOfPaintingIsEnabled
        {
            get => priceOfPaintingIsEnabled;
            set => this.RaiseAndSetIfChanged(ref priceOfPaintingIsEnabled, value);
        }

        public Calculation Calculation { get; } = new Calculation();

        public SparePart SparePart { get; } = new SparePart();

        public string CarImageDescription
        {
            get
            {
                return _carImageName switch
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

        public bool IsPriceEnabled
        {
            get => _isPriceEnabled;
            set => this.RaiseAndSetIfChanged(ref _isPriceEnabled, value);
        }

        public ObservableCollection<DismantlingWork> DismantlingWorks { get; }

        public ObservableCollection<SparePart> SpareParts { get; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> AddSparePartCommand { get; }
        public ReactiveCommand<SparePart, Unit> DeleteSparePartCommand { get; }

        private readonly Car selectedCar_;

        private readonly ICalculationRepository calculationRepository_;

        private readonly ISettingsRepository settingsRepository_;

        private readonly Settings settings_;

        public CalculatorViewModel(IBaseWindow baseWindow, string carImageName)
        {
            BaseWindow = baseWindow;
            _carImageName = carImageName;
            Calculation.ComponentImageName = carImageName;
            Calculation.ComponentName = CarImageDescription;
            CarImage = new Bitmap(ImagePath + carImageName + ".png");
            SaveCommand = ReactiveCommand.CreateFromTask(Save);
            AddSparePartCommand = ReactiveCommand.Create(AddSparePart);
            DeleteSparePartCommand = ReactiveCommand.Create<SparePart>(DeleteSparePart);

            calculationRepository_ = Locator.Current.GetService<ICalculationRepository>();
            var carRepository = Locator.Current.GetService<ICarRepository>();
            var settingsRepository = Locator.Current.GetService<ISettingsRepository>();

            selectedCar_ = carRepository.FindById(carRepository.SelectedCarId);
            settings_ = settingsRepository.FirstOrDefault();

            DismantlingWorks =
                new ObservableCollection<DismantlingWork>(GetDismantlingWorksByBodyType(selectedCar_.BodyType,
                    carImageName));

            SpareParts = new ObservableCollection<SparePart>();
        }

        public CalculatorViewModel(IBaseWindow baseWindow, Calculation calculation)
        {
            BaseWindow = baseWindow;
            _carImageName = calculation.ComponentImageName;
            CarImage = new Bitmap(ImagePath + _carImageName + ".png");
            SaveCommand = ReactiveCommand.CreateFromTask(Save);
            DeleteSparePartCommand = ReactiveCommand.Create<SparePart>(DeleteSparePart);

            calculationRepository_ = Locator.Current.GetService<ICalculationRepository>();
            var carRepository = Locator.Current.GetService<ICarRepository>();
            var settingsRepository = Locator.Current.GetService<ISettingsRepository>();

            selectedCar_ = carRepository.FindById(carRepository.SelectedCarId);
            settings_ = settingsRepository.FirstOrDefault();

            Calculation = calculation;
            Price = calculation.Price;
            DentPriceView = calculation.DentPrice;
            DismantlingPrice = calculation.DismantlingWorks.Where(work => work.IsApplied).Sum(work => work.Price);
            SparePartsPrice = calculation.SpareParts.Sum(part => part.Price * part.Quantity);

            DismantlingWorks = new ObservableCollection<DismantlingWork>(calculation.DismantlingWorks);
            SpareParts = new ObservableCollection<SparePart>(calculation.SpareParts);

            DentDiameterIndex = Calculation.DentDiameter switch
            {
                20 => 0,
                30 => 1,
                _ => 2
            };
            SelectTypeOfRepair(Calculation.TypeOfRepair);
            IsPriceEnabled = Calculation.IsFixPrice;
            NHours = Calculation.NHours;
            RemoveDentPrice = Calculation.RemoveDentPrice;
        }

        private void Calculate()
        {
            double initialDentPrice;
            double currentDentPrice = 0;
            if (!Calculation.IsFixPrice)
            {
                string folder = _carImageName switch
                {
                    "09-1" => @"DentRemoval\roof\",
                    "10-1" => @"DentRemoval\hood\",
                    "11-1" => @"DentRemoval\trunk\",
                    _ => @"DentRemoval\others\"
                };

                var path = ResourcesDirectory + folder + Calculation.DentDiameter + ".json";
                var json = File.ReadAllText(path);
                var sortedList = JsonConvert.DeserializeObject<SortedList<int, double>>(json);
                double hours;
                bool result = sortedList.TryGetValue(Calculation.DentQuantity, out hours);

                if (!result)
                {
                    var keyList = sortedList.Keys.ToList();
                    int keyIndex = keyList.BinarySearch(Calculation.DentQuantity);
                    keyIndex = ~keyIndex - 1;
                    if (keyIndex > -1)
                        hours = sortedList[keyList[keyIndex]];
                    else
                        hours = 0;
                }

                Calculation.NHours = hours;
                NHours = hours;
                Calculation.RemoveDentPrice = settings_.RemoveDentsPrice;
                RemoveDentPrice = settings_.RemoveDentsPrice;
                initialDentPrice = hours * settings_.RemoveDentsPrice;
                currentDentPrice = initialDentPrice;
            }
            else
            {
                Calculation.NHours = NHours;
                Calculation.RemoveDentPrice = settings_.RemoveDentsPrice;
                initialDentPrice = NHours * RemoveDentPrice;
                currentDentPrice = initialDentPrice;
            }

            double currentPrice = 0;
            double sparePartsPrice = 0;
            double dismantlingPrice = 0;

            if (Calculation.TypeOfRepair == TypeOfRepair.UnderPainting)
            {
                currentDentPrice -= initialDentPrice * (settings_.UnderPantingPercent / 100.0);
                currentPrice += Calculation.PriceOfPainting;
            }

            if (Calculation.IsAluminum)
                currentDentPrice += initialDentPrice * (settings_.AluminumPercent / 100.0);

            if (Calculation.IsAdhesive)
                currentDentPrice += initialDentPrice * (settings_.GlueTechniquePercent / 100.0);

            currentPrice += currentDentPrice;

            Calculation.SpareParts = new List<SparePart>(SpareParts);
            sparePartsPrice += SpareParts.Sum(part => part.Price * part.Quantity);
            currentPrice += sparePartsPrice;

            Calculation.DismantlingWorks = new List<DismantlingWork>(DismantlingWorks);

            var appliedWorks = Calculation.DismantlingWorks.Where(work => work.IsApplied).ToList(); 
            dismantlingPrice = appliedWorks.Sum(work => work.Price);
            currentPrice += dismantlingPrice;

            Calculation.IsDismantling = appliedWorks.Count > 0;
            Calculation.DentPrice = currentDentPrice;
            Calculation.Price = currentPrice;
            Price = currentPrice;
            DentPriceView = currentDentPrice;
            DismantlingPrice = dismantlingPrice;
            SparePartsPrice = sparePartsPrice;
        }

        public void SelectTypeOfRepair(TypeOfRepair type)
        {
            PriceOfPaintingIsEnabled = type == TypeOfRepair.UnderPainting;
        }

        public async Task Save()
        {
            Calculate();
            Calculation.CarId = selectedCar_.Id;
            await calculationRepository_.UpdateAsync(Calculation);
            BaseWindow.Content = new CalculationDataViewModel(BaseWindow);
        }

        public void AddSparePart()
        {
            SparePart.CalculationId = Calculation.Id;
            SpareParts.Add(SparePart);
        }

        public void DeleteSparePart(SparePart sparePart)
        {
            SpareParts.Remove(sparePart);
        }

        public void Cancel()
            => BaseWindow.Content = new CalculationDataViewModel(BaseWindow);

        private List<DismantlingWork> GetDismantlingWorksByBodyType(BodyType bodyType, string componentName)
        {
            string jsonFile = GetJsonFileName(componentName);

            if (jsonFile == null)
                return new List<DismantlingWork>();

            string path = @$"{ResourcesDirectory}DismantlingWork\{bodyType}\{jsonFile}";
            string json = File.ReadAllText(path);
            var works = JsonConvert.DeserializeObject<List<DismantlingWork>>(json);
            
            HashSet<string> set = new HashSet<string>
            {
                "01-1",
                "02-1",
                "03-1",
                "04-1",
                "05-1",
                "06-1",
                "09-1",
                "10-1",
                "11-1",
                "12-1",
                "13-1"
            };
            set.Remove(componentName);
            foreach (var item in set)
            {
                jsonFile = GetJsonFileName(item);
                path = @$"{ResourcesDirectory}DismantlingWork\{bodyType}\{jsonFile}";
                json = File.ReadAllText(path);
                var otherWorks = JsonConvert.DeserializeObject<List<DismantlingWork>>(json);
                works.Add(otherWorks);
            }
            
            works.ForEach(work => work.Price = work.NHours * settings_.DismantlingPrice);
            return works;
        }

        private string GetJsonFileName(string componentName)
        {
            string fileName = componentName switch
            {
                "01-1" => "front-left-door.json",
                "02-1" => "front-right-door.json",
                "03-1" => "rear-right-door.json",
                "04-1" => "rear-left-door.json",
                "05-1" => "front-left-fender.json",
                "06-1" => "front-right-fender.json",
                "09-1" => "roof.json",
                "10-1" => "hood.json",
                "11-1" => "truck.json",
                "12-1" => "rear-left-fender.json",
                "13-1" => "rear-right-fender.json",

                // These don't exist
                /*
                 * - Передний бампер
                 * - Задний бампер
                 * - Левая стойка
                 * - Правая стойка.
                 */
                //"07-1" => Localization.LeftRack,
                //"08-1" => Localization.RightRack,
                //"14-1" => Localization.RearBumper,
                _ => null
            };

            return fileName;
        }

        public bool ChangePriceMode(bool value)
        {
            Calculation.IsFixPrice = value;
            return IsPriceEnabled = value;
        }
    }
}