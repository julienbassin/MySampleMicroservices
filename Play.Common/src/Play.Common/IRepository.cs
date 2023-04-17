using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(Guid Id);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetAsync(Guid Id);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task UpdateAsync(T entity);
    }

}