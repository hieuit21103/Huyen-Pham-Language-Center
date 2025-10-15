using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Accounts;

public class CreateUserRequest
{
    public string TenDangNhap { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public string? Avatar { get; set; }
}




