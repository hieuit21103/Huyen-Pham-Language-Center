using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Exceptions;

namespace MsHuyenLC.Application.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    protected readonly IGenericRepository<T> _repository;
    protected readonly IValidator<T>? _validator;

    public GenericService(IGenericRepository<T> repository, IValidator<T>? validator = null)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<T> GetByIdAsync(int id)
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
        // Validation với FluentValidation
        if (_validator != null)
        {
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new Exceptions.ValidationException(errors);
            }
        }

        var result = await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return result;
    }

    public async Task UpdateAsync(T entity)
    {
        // Validation với FluentValidation
        if (_validator != null)
        {
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new Exceptions.ValidationException(errors);
            }
        }

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
