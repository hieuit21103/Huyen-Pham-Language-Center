namespace MsHuyenLC.Application.Interfaces.Auth;

public interface ITokenService
{
    string GeneratePasswordResetToken(string userId);
    bool ValidatePasswordResetToken(string userId, string token);
}