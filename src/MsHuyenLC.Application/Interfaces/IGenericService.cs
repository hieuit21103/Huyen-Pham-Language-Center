using System.Linq.Expressions;

namespace MsHuyenLC.Application.Interfaces;

public interface IGenericService<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<T?> GetByIdAsync(string id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(
        int PageNumber,
        int PageSize,
        Expression<Func<T, bool>>? Filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy = null,
        params Expression<Func<T, object>>[] Includes
    );
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}