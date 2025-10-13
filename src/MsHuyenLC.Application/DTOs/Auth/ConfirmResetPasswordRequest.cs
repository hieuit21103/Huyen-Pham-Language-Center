namespace MsHuyenLC.Application.DTOs.Auth;

public class ConfirmResetPasswordRequest
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string MatKhauMoi { get; set; }
}
