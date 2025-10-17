using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Users.HocVien;

public class HocVienRequest
{
    public string HoTen { get; set; } = null!;
    public DateTime? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public Guid TaiKhoanId { get; set; }
}
