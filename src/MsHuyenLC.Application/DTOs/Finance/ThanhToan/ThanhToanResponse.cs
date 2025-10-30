using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanResponse
{
    public Guid Id { get; set; }
    public string MaThanhToan { get; set; } = null!;
    
    // Thông tin đăng ký
    public Guid DangKyId { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public Guid KhoaHocId { get; set; }
    public string? TenKhoaHoc { get; set; }
    
    // Thông tin thanh toán
    public decimal SoTien { get; set; }
    public decimal? SoTienDaTra { get; set; }
    public decimal? SoTienConLai { get; set; }
    public PhuongThucThanhToan PhuongThuc { get; set; }
    public TrangThaiThanhToan TrangThai { get; set; }
    public string? GhiChu { get; set; }
    
    // Thông tin thanh toán online
    public string? ThongTinNganHang { get; set; }
    public string? MaGiaoDichNganHang { get; set; }
    public string? CongThanhToan { get; set; }
    
    // Thời gian
    public DateOnly NgayLap { get; set; }
    public DateOnly NgayHetHan { get; set; }
    public DateOnly? NgayThanhToan { get; set; }
    public DateTime? NgayXacNhan { get; set; }
    
    // Người xử lý
    public Guid? NguoiXacNhanId { get; set; }
    public string? TenNguoiXacNhan { get; set; }
}
