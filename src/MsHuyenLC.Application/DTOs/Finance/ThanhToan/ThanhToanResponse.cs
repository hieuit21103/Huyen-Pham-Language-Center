using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanResponse
{
    public Guid Id { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public Guid KhoaHocId { get; set; }
    public string? TenKhoaHoc { get; set; }
    public decimal SoTien { get; set; }
    public PhuongThucThanhToan PhuongThuc { get; set; }
    public TrangThaiThanhToan TrangThai { get; set; }
    public string? GhiChu { get; set; }
    public DateTime NgayThanhToan { get; set; }
}
