using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Application.Interfaces.Auth;
using MsHuyenLC.Application.Interfaces.System;
using System.Security.Claims;

namespace MsHuyenLC.API.Controllers.Users;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    protected readonly IUserRepository _userRepository;
    private readonly ISystemLoggerService? _logService;
    
    public ProfileController(IUserRepository userRepository, ISystemLoggerService? logService = null)
    {
        _userRepository = userRepository;
        _logService = logService;
    }
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new 
            { 
                success = false, 
                message = "Chưa đăng nhập" 
            });
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy người dùng" 
            });
        }

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin profile thành công",
            data = user
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] TaiKhoanUpdateRequest request)
    {
        if (request == null)
        {
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ" 
            });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new 
            { 
                success = false, 
                message = "Chưa đăng nhập" 
            });
        }
        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy người dùng" 
            });
        }

        var oldData = new TaiKhoan
        {
            Id = existingUser.Id,
            TenDangNhap = existingUser.TenDangNhap,
            MatKhau = existingUser.MatKhau,
            Email = existingUser.Email,
            Sdt = existingUser.Sdt,
            Avatar = existingUser.Avatar,
            VaiTro = existingUser.VaiTro,
            TrangThai = existingUser.TrangThai
        };

        existingUser.Email = request.Email ?? existingUser.Email;
        existingUser.Sdt = request.Sdt ?? existingUser.Sdt;
        existingUser.Avatar = request.Avatar ?? existingUser.Avatar;

        await _userRepository.UpdateAsync(existingUser);
        await _userRepository.SaveChangesAsync();

        if (_logService != null && Guid.TryParse(userId, out var userGuid))
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            await _logService.LogUpdateAsync(userGuid, oldData, existingUser, ipAddress);
        }

        return Ok(new
        {
            success = true,
            message = "Cập nhật profile thành công",
            data = existingUser
        });
    }
}
