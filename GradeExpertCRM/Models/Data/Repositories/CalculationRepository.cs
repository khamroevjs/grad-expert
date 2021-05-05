using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public class CalculationRepository : Repository<Calculation>, ICalculationRepository
    {
        public IEnumerable<Calculation> GetFullCalculationsByCarId(int id)
        {
            return dbContext_.Calculations
                .Include(x => x.DismantlingWorks)
                .Include(x => x.SpareParts)
                .Where(x => x.CarId == id);
        }
    }
}
