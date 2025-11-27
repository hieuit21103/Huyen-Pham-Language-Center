namespace MsHuyenLC.Application.Interfaces.Services.Auth;

public interface ITokenService
{
    string GeneratePasswordResetToken(string userId);
    bool ValidatePasswordResetToken(string userId, string token);
}