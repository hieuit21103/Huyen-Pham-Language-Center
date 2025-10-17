using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Application.Interfaces.Auth;
using System.Security.Claims;

namespace MsHuyenLC.API.Controllers.Users;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    protected readonly IUserRepository _userRepository;
    public ProfileController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new { message = "Chưa đăng nhập." });
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Người dùng không tồn tại." });
        }

        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] TaiKhoanUpdateRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Dữ liệu không hợp lệ." });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new { message = "Chưa đăng nhập." });
        }
        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            return NotFound(new { message = "Người dùng không tồn tại." });
        }

        existingUser.Email = request.Email ?? existingUser.Email;
        existingUser.Sdt = request.Sdt ?? existingUser.Sdt;
        existingUser.Avatar = request.Avatar ?? existingUser.Avatar;

        await _userRepository.UpdateAsync(existingUser);
        await _userRepository.SaveChangesAsync();

        return Ok(existingUser);
    }
}