using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Application.Interfaces.Services.System;
using System.Security.Claims;

namespace MsHuyenLC.API.Controllers.Users;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ISystemLoggerService? _logService;
    
    public ProfileController(IProfileService profileService, ISystemLoggerService? logService = null)
    {
        _profileService = profileService;
        _logService = logService;
    }
    /// <summary>
    /// Lấy thông tin profile của người dùng đang đăng nhập
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        try
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

            var user = await _profileService.GetProfileAsync(userId);
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
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy thông tin profile",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật thông tin profile của người dùng đang đăng nhập
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] TaiKhoanUpdateRequest request)
    {
        try
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

            // Lấy thông tin cũ để log
            var oldData = await _profileService.GetProfileAsync(userId);
            if (oldData == null)
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Không tìm thấy người dùng" 
                });
            }

            // Cập nhật profile
            var updatedUser = await _profileService.UpdateProfileAsync(userId, request);
            if (updatedUser == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không thể cập nhật profile"
                });
            }

            // Log thay đổi nếu có log service
            if (_logService is not null && Guid.TryParse(userId, out var userGuid))
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                // await _logService.LogUpdateAsync(userGuid, oldData, updatedUser, ipAddress);
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật profile thành công",
                data = updatedUser
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ",
                errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi cập nhật profile",
                error = ex.Message
            });
        }
    }
}

