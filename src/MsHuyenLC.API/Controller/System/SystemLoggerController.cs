using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.System;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SystemLoggerController : BaseController<NhatKyHeThong>
{
    private readonly ISystemLoggerService _logService;

    public SystemLoggerController(
        IGenericService<NhatKyHeThong> service,
        ISystemLoggerService logService) : base(service)
    {
        _logService = logService;
    }

    protected override Func<IQueryable<NhatKyHeThong>, IOrderedQueryable<NhatKyHeThong>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "thoigian" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.ThoiGian))
                : (q => q.OrderBy(k => k.ThoiGian)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [HttpGet("by-user/{taiKhoanId}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetByUser(
        Guid taiKhoanId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var logs = await _service.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
            Filter: log => log.TaiKhoanId == taiKhoanId,
            OrderBy: q => q.OrderByDescending(log => log.ThoiGian)
        );

        return Ok(new
        {
            success = true,
            message = "Lấy nhật ký theo người dùng thành công",
            data = logs
        });
    }

    [HttpGet("by-date-range")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetByDateRange(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var logs = await _service.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
            Filter: log => log.ThoiGian >= fromDate && log.ThoiGian <= toDate,
            OrderBy: q => q.OrderByDescending(log => log.ThoiGian)
        );

        return Ok(new
        {
            success = true,
            message = "Lấy nhật ký theo khoảng thời gian thành công",
            data = logs
        });
    }

    [HttpGet("search")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Search(
        [FromQuery] string? hanhDong = null,
        [FromQuery] string? ip = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var logs = await _service.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
            Filter: log => 
                (string.IsNullOrEmpty(hanhDong) || log.HanhDong.Contains(hanhDong)) &&
                (string.IsNullOrEmpty(ip) || log.IP.Contains(ip)),
            OrderBy: q => q.OrderByDescending(log => log.ThoiGian)
        );

        return Ok(new
        {
            success = true,
            message = "Tìm kiếm nhật ký thành công",
            data = logs
        });
    }
}
