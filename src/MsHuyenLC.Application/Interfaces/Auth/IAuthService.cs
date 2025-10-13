namespace MsHuyenLC.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<string> Login(string username, string password);
    Task Logout(string userId);
    Task<bool> IsUserLoggedIn(string userId);
    Task<bool> ResetPassword(string email,string token, string newPassword);
    Task<bool> ChangePassword(string userId, string currentPassword, string newPassword);
}