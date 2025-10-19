using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKy;

public class DangKyCreateRequest
{
    public Guid KhoaHocId { get; set; }
    public Guid HocVienId { get; set; }
    public TrangThaiDangKy TrangThai { get; set; }
    public DateTime? NgayDangKy { get; set; }
    public Guid? LopHocId { get; set; }
    public DateTime? NgayXepLop { get; set; }
}