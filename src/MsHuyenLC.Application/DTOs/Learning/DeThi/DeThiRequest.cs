using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DeThi;

public class DeThiRequest
{
    public Guid Id { get; set; }
    public string TenDe { get; set; } = null!;
    public int TongCauHoi { get; set; }
    public string? KyThiId { get; set; }
    public int ThoiGianLamBai { get; set; }
    public Guid? NguoiTaoId { get; set; }
    public List<string> CauHoiIds { get; set; } = new List<string>();  
}