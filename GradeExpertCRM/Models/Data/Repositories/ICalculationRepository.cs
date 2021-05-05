using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Source;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public interface ICalculationRepository : IRepository<Calculation>
    {
        public IEnumerable<Calculation> GetFullCalculationsByCarId(int id);
    }
}
