using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public class CarRepository : Repository<Car>, ICarRepository
    {
        public int SelectedCarId { get; set; }

        public Car CarWithCalculations(int id)
        {
            return dbContext_.Cars.Include(x => x.Calculations).FirstOrDefault(x => x.Id == id);
        }

        public Car GetFullCar(int id)
        {
            return dbContext_.Cars
                .Include(x => x.Client)
                .Include(x => x.OverallCalculation)
                .Include(x => x.Calculations).ThenInclude(x => x.DismantlingWorks)
                .Include(x => x.Calculations).ThenInclude(x => x.SpareParts)
                .FirstOrDefault(x => x.Id == id);
        }
        
        public async Task<Car> GetFullCarAsync(int id)
        {
            return await dbContext_.Cars
                .Include(x => x.Client)
                .Include(x => x.OverallCalculation)
                .Include(x => x.Calculations).ThenInclude(x => x.DismantlingWorks)
                .Include(x => x.Calculations).ThenInclude(x => x.SpareParts)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}