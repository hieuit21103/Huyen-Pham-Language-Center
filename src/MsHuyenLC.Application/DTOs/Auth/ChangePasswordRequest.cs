namespace MsHuyenLC.Application.DTOs.Auth;

public class ChangePasswordRequest
{
    public required string MatKhauCu { get; set; }
    public required string MatKhauMoi { get; set; }
}
