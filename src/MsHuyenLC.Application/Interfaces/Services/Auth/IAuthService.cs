using MsHuyenLC.Application.DTOs.Auth;

namespace MsHuyenLC.Application.Interfaces.Services.Auth;

public interface IAuthService
{
    Task<string> Login(LoginRequest request);
    Task Logout(string userId);
    Task<bool> IsUserLoggedIn(string userId);
    Task<bool> ResetPassword(ConfirmResetPasswordRequest request);
    Task<bool> ChangePassword(ChangePasswordRequest request, string userId);
}