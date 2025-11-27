using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanResponse
{
    public Guid Id { get; set; }
    public string? MaThanhToan { get; set; }
    public Guid DangKyId { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public Guid KhoaHocId { get; set; }
    public string? TenKhoaHoc { get; set; }
    public decimal SoTien { get; set; }
    public PhuongThucThanhToan PhuongThuc { get; set; }
    public TrangThaiThanhToan TrangThai { get; set; }
    public string? GhiChu { get; set; }
    public string? ThongTinNganHang { get; set; }
    public string? MaGiaoDichNganHang { get; set; }
    public string? CongThanhToan { get; set; }
    public DateOnly NgayLap { get; set; }
    public DateOnly NgayHetHan { get; set; }
    public DateOnly? NgayThanhToan { get; set; }
}
