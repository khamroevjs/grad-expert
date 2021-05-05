using System.Collections.Generic;
using GradeExpertCRM.Models;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using GradeExpertCRM.Models.Data.Repositories;
using Splat;
using System.Linq;

namespace GradeExpertCRM.ViewModels.Frames
{
    internal class CarViewModel : ViewModelBase
    {
        private string _searchString;

        private ObservableCollection<Car> _allCars;

        private ObservableCollection<Car> _cars;

        public ObservableCollection<Car> Cars
        {
            get => _cars;
            set => this.RaiseAndSetIfChanged(ref _cars, value);
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchString, value);
                if (_allCars != null)
                {
                    var cars = _allCars.Where(car => car.Model.Contains(_searchString));
                    Cars = new ObservableCollection<Car>(cars);
                }
            }
        }

        private async Task OpenAddingCarView() => BaseWindow.Content = new AddingCarViewModel(BaseWindow);

        public ReactiveCommand<Unit, Unit> GoAddingCarView { get; }

        private ICarRepository carRepository_;
        private ICalculationRepository calculationRepository_;
        public Car SelectedCarId
        {
            set
            {
                if (value != null)
                    carRepository_.SelectedCarId = value.Id;
            }
        }
        
        public ReactiveCommand<Car, Unit> ChangeCommand { get; }
        public ReactiveCommand<Car, Unit> DeleteCommand { get; }

        public CarViewModel(IBaseWindow baseWindow)
        {
            BaseWindow = baseWindow;
            GoAddingCarView = ReactiveCommand.CreateFromTask(OpenAddingCarView);
            ChangeCommand = ReactiveCommand.CreateFromTask<Car>(Change);
            DeleteCommand = ReactiveCommand.CreateFromTask<Car>(Delete);
            carRepository_ = Locator.Current.GetService<ICarRepository>();
            calculationRepository_ = Locator.Current.GetService<ICalculationRepository>();
            var cars = carRepository_.All();
            _allCars = new ObservableCollection<Car>(cars);
            Cars = _allCars;
        }
        
        private async Task Change(Car car)
        {
            BaseWindow.Content = new AddingCarViewModel(BaseWindow, car);
        }
        
        private async Task Delete(Car car)
        {
            Cars.Remove(car);
            await carRepository_.RemoveAsync(car);
        }
    }
}