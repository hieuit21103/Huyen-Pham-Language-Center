using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces.Auth;
using MsHuyenLC.Application.DTOs.Auth;
using System.Security.Claims;
using MsHuyenLC.Application.Interfaces.Email;

namespace MsHuyenLC.API.Controller.Auth;

[Route("api/[controller]")]
[ApiController]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public AuthController(IAuthService authService, ITokenService tokenService, IUserRepository userRepository, IEmailService emailService)
    {
        _authService = authService;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var username = loginRequest.TenDangNhap;
        var password = loginRequest.MatKhau;

        var result = await _authService.Login(username, password);
        if (result == "") return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new { message = "Chưa đăng nhập" });

        await _authService.Logout(userId);
        return Ok(new { message = "Đăng xuất thành công" });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new { message = "Chưa đăng nhập" });

        var result = await _authService.ChangePassword(userId, request.MatKhauCu, request.MatKhauMoi);
        if (!result)
            return BadRequest(new { message = "Đổi mật khẩu thất bại" });

        return Ok(new { message = "Đổi mật khẩu thành công" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return NotFound(new { message = "Không tìm thấy người dùng với email này" });

        var token = _tokenService.GeneratePasswordResetToken(user.Id.ToString());

        var resetLink = $"{request.ReturnUrl}?token={token}&email={request.Email}";
        await _emailService.SendPasswordResetEmailAsync(request.Email, user.TenDangNhap, resetLink, 30);

        return Ok(new { message = "Gửi email đặt lại mật khẩu thành công" });
    }
    
    [HttpPost("reset-password/confirm")]
    public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.ResetPassword(request.Email,request.Token, request.MatKhauMoi);
        if (!result)
            return BadRequest(new { message = "Đặt lại mật khẩu thất bại. Token không hợp lệ hoặc đã hết hạn." });

        return Ok(new { message = "Đặt lại mật khẩu thành công" });
    }
}
