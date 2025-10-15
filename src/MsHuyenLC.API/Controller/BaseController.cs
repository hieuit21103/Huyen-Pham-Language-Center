using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using System.Linq.Expressions;

namespace MsHuyenLC.API.Controller;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController<TEntity> : ControllerBase where TEntity : class
{
    protected readonly IGenericService<TEntity> _service;
    public BaseController(IGenericService<TEntity> service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all entities with pagination
    /// Example: GET /api/courses?pageNumber=1&pageSize=10&sortBy=name&sortOrder=asc
    /// </summary>
    [HttpGet]
    public virtual async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "asc"
    )
    {
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null;
        
        if (!string.IsNullOrEmpty(sortBy))
        {
            orderBy = BuildOrderBy(sortBy, sortOrder);
        }
        
        var entities = await _service.GetAllAsync(pageNumber, pageSize, null, orderBy);
        return Ok(entities);
    }

    /// <summary>
    /// Override this method in derived controllers to support dynamic sorting
    /// </summary>
    protected virtual Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return null;
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }
}