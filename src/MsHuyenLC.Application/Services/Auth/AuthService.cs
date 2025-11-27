using System.Linq;
using System.Security.Cryptography;
using FluentValidation;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Auth;
using MsHuyenLC.Application.DTOs.Auth;

namespace MsHuyenLC.Application.Services;

public class AuthService : IAuthService
{
    protected readonly ITokenService _tokenService;
    protected readonly IPasswordHasher _passwordHasher;
    protected readonly IJwtService _jwtService;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IValidator<LoginRequest> _loginValidator;
    protected readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
    protected readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
    protected readonly IValidator<ConfirmResetPasswordRequest> _confirmResetPasswordValidator;
    public AuthService(
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        IValidator<LoginRequest> loginValidator,
        IValidator<ResetPasswordRequest> resetPasswordValidator,
        IValidator<ChangePasswordRequest> changePasswordValidator,
        IValidator<ConfirmResetPasswordRequest> confirmResetPasswordValidator)
    {
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _loginValidator = loginValidator;
        _resetPasswordValidator = resetPasswordValidator;
        _changePasswordValidator = changePasswordValidator;
        _confirmResetPasswordValidator = confirmResetPasswordValidator;
    }
    public async Task<string> Login(LoginRequest request)
    {
        await _loginValidator.ValidateAndThrowAsync(request);

        var user = await _unitOfWork.TaiKhoans.GetByUsernameAsync(request.TenDangNhap);
        if (user == null || !_passwordHasher.VerifyPassword(user.MatKhau, request.MatKhau))
        {
            return "";
        }

        return _jwtService.GenerateToken(user.Id.ToString(), user.Email ?? "Không có email", new[] { user.VaiTro.ToString() });
    }

    public async Task Logout(string userId)
    {
        await Task.CompletedTask;
    }

    public async Task<bool> IsUserLoggedIn(string userId)
    {
        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        return user != null;
    }

    public async Task<bool> ResetPassword(ConfirmResetPasswordRequest request)
    {
        await _confirmResetPasswordValidator.ValidateAndThrowAsync(request);

        var user = await _unitOfWork.TaiKhoans.GetByEmailAsync(request.Email);
        if (user == null) return false;
        var validToken = _tokenService.ValidatePasswordResetToken(user.Id.ToString(), request.Token);
        if (!validToken) return false;
        user.MatKhau = _passwordHasher.HashPassword(request.MatKhauMoi);
        await _unitOfWork.TaiKhoans.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePassword(ChangePasswordRequest request, string userId)
    {
        await _changePasswordValidator.ValidateAndThrowAsync(request);
        {
            var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
            if (user == null || !_passwordHasher.VerifyPassword(user.MatKhau, request.MatKhauCu))
            {
                return false;
            }

            user.MatKhau = _passwordHasher.HashPassword(request.MatKhauMoi);
            await _unitOfWork.TaiKhoans.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}