using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Accounts;

public class UpdateUserRequest
{
    public VaiTro VaiTro { get; set; } = VaiTro.hocvien;
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public TrangThaiTaiKhoan TrangThai { get; set; } = TrangThaiTaiKhoan.hoatdong;
    public string? Avatar { get; set; }
}




