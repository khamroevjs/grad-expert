using System.Threading.Tasks;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public interface ICarRepository : IRepository<Car>
    {
        public int SelectedCarId { get; set; }

        public Car CarWithCalculations(int id);

        public Car GetFullCar(int id);

        public Task<Car> GetFullCarAsync(int id);
    }
}
