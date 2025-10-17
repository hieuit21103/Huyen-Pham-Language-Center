using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanRequest
{
    public Guid HocVienId { get; set; }
    public Guid KhoaHocId { get; set; }
    public decimal SoTien { get; set; }
    public PhuongThucThanhToan PhuongThuc { get; set; }
    public string? GhiChu { get; set; }
}
