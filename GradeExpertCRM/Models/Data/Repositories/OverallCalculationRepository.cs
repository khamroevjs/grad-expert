using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public class OverallCalculationRepository : Repository<OverallCalculation>, IOverallCalculationRepository
    {
        public OverallCalculation GetByCarId(int id)
        {
            return dbContext_.OverallCalculations.FirstOrDefault(x => x.CarId == id);
        }
    }
}
