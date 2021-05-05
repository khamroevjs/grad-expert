using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avalonia.Data;
using Common.Logging.Configuration;
using Microsoft.EntityFrameworkCore;
using Splat;

namespace GradeExpertCRM.Models.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        protected readonly AppDbContext dbContext_;

        public Repository()
        {
            dbContext_ = Locator.Current.GetService<AppDbContext>();
        }

        public IEnumerable<T> All()
        {
            return dbContext_.Set<T>().ToList();
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return dbContext_.Set<T>().Where(predicate);
        }

        public T Add(T item)
        {
            dbContext_.Add(item);
            dbContext_.SaveChanges();
            return item;
        }

        public async Task<T> AddAsync(T item) //TODO May cause exception!!!
        {
            await dbContext_.AddAsync(item);
            await dbContext_.SaveChangesAsync();
            return item;
        }

        public async Task<T> FindByIdAsync(int id)
        {
            return await dbContext_.FindAsync<T>(id);
        }

        public T FindById(int id)
        {
            return dbContext_.Find<T>(id);
        }

        public void Remove(T item)
        {
            dbContext_.Remove(item);
            dbContext_.SaveChanges();
        }

        public T Update(T item)
        {
            dbContext_.Update(item);
            dbContext_.SaveChanges();
            return item;
        }

        public async Task RemoveAsync(T item)
        {
            dbContext_.Remove(item);
            await dbContext_.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T item)
        {
            dbContext_.Update(item);
            await dbContext_.SaveChangesAsync();
            return item;
        }

        public T FirstOrDefault()
        {
            return dbContext_.Set<T>().FirstOrDefault();
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            return await dbContext_.Set<T>().FirstOrDefaultAsync();
        }
    }
}
