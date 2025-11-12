using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.System;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly ISystemLoggerService _logService;

    public BaseController(ISystemLoggerService logService)
    {
        _logService = logService;
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

    protected async Task LogCreateAsync(object entity)
    {
        if (_logService != null)
        {
            await _logService.LogCreateAsync(GetCurrentUserId(), entity, GetClientIpAddress());
            await _logService.SaveChangesAsync();
        }
    }

    protected async Task LogUpdateAsync(object oldEntity, object newEntity)
    {
        if (_logService != null)
        {
            await _logService.LogUpdateAsync(GetCurrentUserId(), oldEntity, newEntity, GetClientIpAddress());
            await _logService.SaveChangesAsync();
        }
    }

    protected async Task LogDeleteAsync(object entity)
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

