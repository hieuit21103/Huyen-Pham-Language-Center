using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Users.TaiKhoan;

public class TaiKhoanUpdateRequest
{
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public string? Avatar { get; set; }
    public VaiTro VaiTro { get; set; }
    public TrangThaiTaiKhoan TrangThai { get; set; }
}
