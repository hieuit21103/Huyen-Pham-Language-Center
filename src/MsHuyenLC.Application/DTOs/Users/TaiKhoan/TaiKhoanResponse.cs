using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Users.TaiKhoan;

public class TaiKhoanResponse
{
    public Guid Id { get; set; }
    public string TenDangNhap { get; set; } = null!;
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public VaiTro VaiTro { get; set; }
    public TrangThaiTaiKhoan TrangThai { get; set; }
    public string? Avatar { get; set; }
    public DateTime NgayTao { get; set; }
}
