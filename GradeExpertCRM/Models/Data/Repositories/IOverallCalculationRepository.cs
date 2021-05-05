using System;
using System.Collections.Generic;
using System.Text;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public interface IOverallCalculationRepository : IRepository<OverallCalculation>
    {
        public OverallCalculation GetByCarId(int id);
    }
}
