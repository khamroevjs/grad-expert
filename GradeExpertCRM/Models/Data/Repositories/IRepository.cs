using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public interface IRepository<T> where T : class, new()
    {
        IEnumerable<T> All();
        T FirstOrDefault();
        Task<T> FirstOrDefaultAsync();
        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);
        T Add(T item);
        Task<T> AddAsync(T item);
        Task<T> FindByIdAsync(int id);
        T FindById(int id);
        void Remove(T item);
        T Update(T item);
        Task RemoveAsync(T item);
        Task<T> UpdateAsync(T item);
    }
}
