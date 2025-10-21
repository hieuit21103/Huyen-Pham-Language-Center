using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.ThongBao;

public class ThongBaoResponse
{
    public Guid Id { get; set; }
    public Guid NguoiGui { get; set; }
    public string? TenNguoiGui { get; set; }
    public Guid? LopHocId { get; set; }
    public string? TenLop { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public DoiTuongNhan DoiTuongNhan { get; set; }
    public DateOnly NgayGui { get; set; }
}
