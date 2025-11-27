using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Courses.KhoaHoc;

public class KhoaHocUpdateRequest
{
    public string TenKhoaHoc { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal HocPhi { get; set; }
    public int ThoiLuong { get; set; }
    public DateOnly NgayKhaiGiang { get; set; }
    public TrangThaiKhoaHoc TrangThai { get; set; }
}
