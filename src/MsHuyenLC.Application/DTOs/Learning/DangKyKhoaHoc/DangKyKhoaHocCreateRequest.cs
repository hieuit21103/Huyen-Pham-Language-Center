using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKyKhoaHoc;

public class DangKyKhoaHocCreateRequest
{
    public Guid KhoaHocId { get; set; }
    public Guid HocVienId { get; set; }
    public TrangThaiDangKy TrangThai { get; set; }
    public DateOnly? NgayDangKy { get; set; }
    public Guid? LopHocId { get; set; }
    public DateOnly? NgayXepLop { get; set; }
}