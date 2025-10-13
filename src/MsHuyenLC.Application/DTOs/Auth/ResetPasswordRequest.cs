namespace MsHuyenLC.Application.DTOs.Auth;

public class ResetPasswordRequest
{
    public required string Email { get; set; }
    public required string ReturnUrl { get; set; }
}
