using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKy;

public class DangKyHocVienResponse
{
    public Guid Id { get; set; }
    public KhoaHoc? KhoaHoc { get; set; }
    public LopHoc? LopHoc { get; set; }
    public DateOnly NgayDangKy { get; set; }
    public TrangThaiDangKy? TrangThai { get; set; }
}