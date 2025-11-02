using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Finance.ThanhToan;

public class ThanhToanRequest
{
    public Guid DangKyId { get; set; }
    public decimal SoTien { get; set; }
}
