using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces.Auth;
using MsHuyenLC.Application.DTOs.Auth;
using System.Security.Claims;
using MsHuyenLC.Application.Interfaces.Email;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Auth;

[Route("api/[controller]")]
[ApiController]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ISystemLoggerService _logService;

    public AuthController(
        IAuthService authService,
        ITokenService tokenService,
        IUserRepository userRepository,
        IEmailService emailService,
        ISystemLoggerService logService)
    {
        _authService = authService;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _emailService = emailService;
        _logService = logService;
    }

    /// <summary>
    /// Đăng nhập người dùng
    /// <param name="request">Thông tin đăng nhập</param>
    /// <returns>Token JWT nếu đăng nhập thành công</returns>
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(
            new { 
                success= false,
                message = "Dữ liệu không hợp lệ",
                errors = ModelState
            });

        var username = request.TenDangNhap;
        var password = request.MatKhau;

        var result = await _authService.Login(username, password);
        
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user != null)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            await _logService.LogLoginAsync(user.Id, ipAddress, result != "");
        }

        if (result == "") {
            return Unauthorized(new
            {
                success = false,
                message = "Sai tài khoản hoặc mật khẩu"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Đăng nhập thành công",
            token = result
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new
            {
                success = false,
                message = "Chưa đăng nhập"
            });

        await _authService.Logout(userId);
        
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _logService.LogLogoutAsync(Guid.Parse(userId), ipAddress);

        return Ok(new
        {
            success = true,
            message = "Đăng xuất thành công"
        });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new
            {
                success = false,
                message = "Chưa đăng nhập"
            });

        var result = await _authService.ChangePassword(userId, request.MatKhauCu, request.MatKhauMoi);
        if (!result)
            return BadRequest(new
            {
                success = false,
                message = "Đổi mật khẩu thất bại. Mật khẩu cũ không đúng."
            });

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _logService.LogAsync(
            Guid.Parse(userId),
            "Đổi mật khẩu",
            "Người dùng đã thay đổi mật khẩu thành công",
            "",
            "",
            ipAddress);

        return Ok(new
        {
            success = true,
            message = "Đổi mật khẩu thành công"
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy người dùng với email này" 
            });

        var token = _tokenService.GeneratePasswordResetToken(user.Id.ToString());

        var resetLink = $"{request.ReturnUrl}?token={token}&email={request.Email}";
        await _emailService.SendPasswordResetEmailAsync(request.Email, user.TenDangNhap, resetLink, 30);

        return Ok(new 
        { 
            success = true, 
            message = "Gửi email đặt lại mật khẩu thành công" 
        });
    }

    [HttpPost("reset-password/confirm")]
    public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var result = await _authService.ResetPassword(request.Email, request.Token, request.MatKhauMoi);
        if (!result)
            return BadRequest(new 
            { 
                success = false, 
                message = "Đặt lại mật khẩu thất bại. Token không hợp lệ hoặc đã hết hạn." 
            });

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user != null)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            await _logService.LogAsync(
                user.Id,
                "Đặt lại mật khẩu",
                "Người dùng đã đặt lại mật khẩu thành công qua email",
                "",
                "",
                ipAddress);
        }

        return Ok(new 
        { 
            success = true, 
            message = "Đặt lại mật khẩu thành công" 
        });
    }
}
