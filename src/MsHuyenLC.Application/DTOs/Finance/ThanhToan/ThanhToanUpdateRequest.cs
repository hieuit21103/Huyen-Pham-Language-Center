using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanUpdateRequest
{
    public TrangThaiThanhToan TrangThai { get; set; }
    public string? MaThanhToan { get; set; }
    public string? GhiChu { get; set; }
    public string? ThongTinNganHang { get; set; }
    public string? MaGiaoDichNganHang { get; set; }
    public string? CongThanhToan { get; set; }
    public DateOnly? NgayThanhToan { get; set; }
}
