using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Courses.LopHoc;

public class LopHocResponse
{
    public Guid Id { get; set; }
    public string TenLop { get; set; } = null!;
    public Guid KhoaHocId { get; set; }
    public string? TenKhoaHoc { get; set; }
    public int SiSoHienTai { get; set; }
    public int SiSoToiDa { get; set; }
    public TrangThaiLopHoc TrangThai { get; set; }
}
