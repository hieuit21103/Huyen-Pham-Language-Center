using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Exceptions;

namespace MsHuyenLC.Application.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    protected readonly IGenericRepository<T> _repository;

    public GenericService(IGenericRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<T> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        int PageNumber,
        int PageSize,
        Expression<Func<T, bool>>? Filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy = null,
        params Expression<Func<T, object>>[] Includes
    )
    {
        return await _repository.GetAllAsync(PageNumber, PageSize, Filter, OrderBy, Includes);
    }

    public async Task<T> AddAsync(T entity)
    {
        var result = await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return result;
    }

    public async Task UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        await _repository.DeleteAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _repository.ExistsAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return await _repository.CountAsync(predicate);
    }
}
