using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanUpdateRequest
{
    public TrangThaiThanhToan TrangThai { get; set; }
    public string? GhiChu { get; set; }
}
