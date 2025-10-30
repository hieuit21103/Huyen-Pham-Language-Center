using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanRequest
{
    public Guid DangKyId { get; set; }
    public decimal SoTien { get; set; }
    public decimal? SoTienDaTra { get; set; } // Cho trường hợp trả góp
    public PhuongThucThanhToan PhuongThuc { get; set; }
    public string? GhiChu { get; set; }
    
    // Thông tin thanh toán online
    public string? ThongTinNganHang { get; set; }
    public string? MaGiaoDichNganHang { get; set; }
    public string? CongThanhToan { get; set; } // VNPay, MoMo, ZaloPay
}
