using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Students;

public class StudentUpdateRequest
{
    public string? HoTen { get; set; }
    public DateTime? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public TrangThaiHocVien? TrangThai { get; set; }
}