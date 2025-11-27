using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DeThi;

public class DeThiUpdateRequest
{
    public string TenDe { get; set; } = null!;
    public int TongCauHoi { get; set; }
    public int ThoiGianLamBai { get; set; }
    public Guid? KyThiId { get; set; }
    public List<string> CauHoiIds { get; set; } = new List<string>();
}
