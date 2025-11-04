using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DeThi;

public class CreateMixedTestRequest
{
    public string TenDe { get; set; } = null!;
    public int ThoiGianLamBai { get; set; }
    public LoaiDeThi LoaiDeThi { get; set; } = LoaiDeThi.LuyenTap;
    public string? KyThiId { get; set; }
    public List<string> NhomCauHoiIds { get; set; } = new List<string>();
    public List<string> CauHoiDocLapIds { get; set; } = new List<string>();
}
