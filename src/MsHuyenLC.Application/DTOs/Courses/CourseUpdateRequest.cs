using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Domain.Entities.Finance;

namespace MsHuyenLC.Application.DTOs.Courses;
public class CourseUpdateRequest
{
    public string TenKhoaHoc { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal HocPhi { get; set; }
    public int ThoiLuong { get; set; }
    public DateTime NgayKhaiGiang { get; set; }
    public TrangThaiKhoaHoc TrangThai { get; set; } = TrangThaiKhoaHoc.dangmo;

    public ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}