using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Users.HocVien;

public class HocVienUpdateRequest
{
    public string HoTen { get; set; } = null!;
    public DateOnly? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public TrangThaiHocVien TrangThai { get; set; }
}
