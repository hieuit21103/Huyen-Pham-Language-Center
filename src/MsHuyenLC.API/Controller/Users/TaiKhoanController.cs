using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces.Services.Auth;
using System.Security.Claims;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.User;

namespace MsHuyenLC.API.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class TaiKhoanController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISystemLoggerService? _logService;

    public TaiKhoanController(
        IUserService userService,
        IPasswordHasher passwordHasher,
        ISystemLoggerService? logService = null)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
        _logService = logService;
    }

    protected Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    protected string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        if (string.IsNullOrEmpty(id)) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Tài khoản không hợp lệ" 
            });

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy tài khoản" 
            });

        return Ok(new
        {
            success = true,
            data = user
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách thành công",
            data = users
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] TaiKhoanRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        if (request == null) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ" 
            });

        var createdUser = await _userService.CreateUserAsync(request);

        if (_logService != null && GetCurrentUserId() != null)
        {
            await _logService.LogCreateAsync(GetCurrentUserId()!.Value, createdUser, GetClientIpAddress());
        }

        return Ok(new
        {
            success = true,
            message = "Tạo tài khoản thành công",
            data = createdUser
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] TaiKhoanUpdateRequest request)
    {
        if (string.IsNullOrEmpty(id) || request == null)
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ" 
            });

        if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != id && !User.IsInRole("admin"))
            return Forbid();

        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy tài khoản" 
            });

        var updatedUser = await _userService.UpdateUserAsync(id, request);

        if (_logService != null && GetCurrentUserId() != null)
        {
            await _logService.LogUpdateAsync(GetCurrentUserId()!.Value, existingUser, updatedUser, GetClientIpAddress());
        }

        return Ok(new
        {
            success = true,
            message = "Cập nhật tài khoản thành công",
            data = updatedUser
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id)) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Tài khoản không hợp lệ" 
            });

        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy tài khoản" 
            });

        await _userService.DeleteUserAsync(id);

        if (_logService != null && GetCurrentUserId() != null)
        {
            await _logService.LogDeleteAsync(GetCurrentUserId()!.Value, existingUser, GetClientIpAddress());
        }

        return Ok(new 
        { 
            success = true, 
            message = "Xóa tài khoản thành công" 
        });
    }
}

