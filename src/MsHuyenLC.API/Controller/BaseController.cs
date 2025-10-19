using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController<TEntity> : ControllerBase where TEntity : class
{
    protected readonly IGenericService<TEntity> _service;
    protected readonly ISystemLoggerService? _logService;

    public BaseController(IGenericService<TEntity> service)
    {
        _service = service;
        _logService = null;
    }

    public BaseController(IGenericService<TEntity> service, ISystemLoggerService logService)
    {
        _service = service;
        _logService = logService;
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

    #region Logging Helper Methods

    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    protected string GetClientIpAddress()
    {
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').FirstOrDefault()?.Trim() ?? "Unknown";
        }
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    protected async Task LogCreateAsync(TEntity entity)
    {
        if (_logService != null)
        {
            await _logService.LogCreateAsync(GetCurrentUserId(), entity, GetClientIpAddress());
            await _logService.SaveChangesAsync();
        }
    }

    protected async Task LogUpdateAsync(TEntity oldEntity, TEntity newEntity)
    {
        if (_logService != null)
        {
            await _logService.LogUpdateAsync(GetCurrentUserId(), oldEntity, newEntity, GetClientIpAddress());
            await _logService.SaveChangesAsync();
        }
    }

    protected async Task LogDeleteAsync(TEntity entity)
    {
        if (_logService != null)
        {
            await _logService.LogDeleteAsync(GetCurrentUserId(), entity, GetClientIpAddress());
            await _logService.SaveChangesAsync();
        }
    }

    protected async Task LogAsync(string hanhDong, string? chiTiet = null, string duLieuCu = "", string duLieuMoi = "")
    {
        if (_logService != null)
        {
            await _logService.LogAsync(
                GetCurrentUserId(),
                hanhDong,
                chiTiet,
                duLieuCu,
                duLieuMoi,
                GetClientIpAddress()
            );
            await _logService.SaveChangesAsync();
        }
    }

    #endregion
}
