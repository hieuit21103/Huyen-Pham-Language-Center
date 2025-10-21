using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKy;

public class DangKyResponse
{
    public Guid Id { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public Guid KhoaHocId { get; set; }
    public string? TenKhoaHoc { get; set; }
    public Guid? LopHocId { get; set; }
    public string? TenLop { get; set; }
    public DateOnly NgayDangKy { get; set; }
    public DateOnly? NgayXepLop { get; set; }
    public TrangThaiDangKy TrangThai { get; set; }
}
