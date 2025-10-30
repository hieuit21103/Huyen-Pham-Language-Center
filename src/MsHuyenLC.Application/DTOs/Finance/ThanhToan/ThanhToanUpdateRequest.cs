using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanUpdateRequest
{
    public TrangThaiThanhToan TrangThai { get; set; }
    public decimal? SoTienDaTra { get; set; }
    public string? GhiChu { get; set; }
    
    // Thông tin thanh toán online
    public string? ThongTinNganHang { get; set; }
    public string? MaGiaoDichNganHang { get; set; }
    public string? CongThanhToan { get; set; }
    
    // Người xác nhận (được set từ backend)
    public DateOnly? NgayThanhToan { get; set; }
}
