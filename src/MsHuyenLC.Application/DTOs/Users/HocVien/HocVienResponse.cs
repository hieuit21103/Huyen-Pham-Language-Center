using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Users.HocVien;

public class HocVienResponse
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public DateOnly? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public Guid TaiKhoanId { get; set; }
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public TrangThaiHocVien TrangThai { get; set; }
    public DateOnly NgayDangKy { get; set; }
}
