using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Users.TaiKhoan;

public class TaiKhoanRequest
{
    public string TenDangNhap { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public VaiTro VaiTro { get; set; }
    public string? Avatar { get; set; }
}
