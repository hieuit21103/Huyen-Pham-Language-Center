using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MsHuyenLC.Application.Interfaces.Auth;

namespace MsHuyenLC.Infrastructure.Services;

public class AuthService : IAuthService
{
    protected readonly ITokenService _tokenService;
    protected readonly IPasswordHasher _passwordHasher;
    protected readonly IJwtService _jwtService;
    protected readonly IUserRepository _userRepository;
    public AuthService(ITokenService tokenService, IPasswordHasher passwordHasher, IJwtService jwtService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _userRepository = userRepository;
    }
    public async Task<string> Login(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !_passwordHasher.VerifyPassword(user.MatKhau, password))
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
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null;
    }

    public async Task<bool> ResetPassword(string email, string token, string newPassword)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;
        var validToken = _tokenService.ValidatePasswordResetToken(user.Id.ToString(), token);
        if (!validToken) return false;
        user.MatKhau = _passwordHasher.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePassword(string userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !_passwordHasher.VerifyPassword(user.MatKhau, currentPassword))
        {
            return false;
        }

        user.MatKhau = _passwordHasher.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }
}