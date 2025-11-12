using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.System;

namespace MsHuyenLC.API.Controller.System;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SystemLoggerController : BaseController<NhatKyHeThong>
{
    public SystemLoggerController(
        ISystemLoggerService logService) : base(logService)
    {
    }

    [HttpGet("by-user/{taiKhoanId}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetByUser(Guid taiKhoanId)
    {
        var logs = await _logService.GetByUserIdAsync(taiKhoanId);

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
        [FromQuery] DateOnly toDate)
    {
        var logs = await _logService.GetByDateRangeAsync(fromDate, toDate);

        return Ok(new
        {
            success = true,
            message = "Lấy nhật ký theo khoảng thời gian thành công",
            data = logs
        });
    }
}


