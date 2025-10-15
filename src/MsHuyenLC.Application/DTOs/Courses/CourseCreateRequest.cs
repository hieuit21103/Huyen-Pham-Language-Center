using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Courses;

public class CourseCreateRequest
{
    public string TenKhoaHoc { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal HocPhi { get; set; }
    public int ThoiLuong { get; set; }
    public DateTime NgayKhaiGiang { get; set; }
    public TrangThaiKhoaHoc TrangThai { get; set; } = TrangThaiKhoaHoc.dangmo;
}

