using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Domain.Entities.Learning;

public class DeThiRequest
{
    public Guid Id { get; set; }
    public string TenDe { get; set; } = null!;
    public int SoCauHoi { get; set; }
    public LoaiDeThi LoaiDeThi { get; set; } = LoaiDeThi.LuyenTap;
    public Guid? KyThiId { get; set; }
    public KyThi? KyThi { get; set; }
    public int ThoiGianLamBai { get; set; }
    public List<string> CauHoiIds { get; set; } = new List<string>();  
}