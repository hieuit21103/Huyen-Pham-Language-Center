using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Infrastructure.Persistence;

namespace MsHuyenLC.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _dbSet.FindAsync(Guid.Parse(id));
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        int PageNumber,
        int PageSize,
        Expression<Func<T, bool>>? Filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy = null,
        params Expression<Func<T, object>>[] Includes
    )
    {
        IQueryable<T> query = _dbSet;

        if (Filter != null)
        {
            query = query.Where(Filter);
        }

        foreach (var include in Includes)
        {
            query = query.Include(include);
        }

        if (OrderBy != null)
        {
            query = OrderBy(query);
        }

        return await query.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate != null)
        {
            return await _dbSet.CountAsync(predicate);
        }
        return await _dbSet.CountAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}