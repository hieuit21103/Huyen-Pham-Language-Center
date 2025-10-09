namespace MsHuyenLC.Domain.Entities.Learning;

public class DeThi
{
    public Guid Id { get; set; }
    public string TenDe { get; set; } = null!;
    public int SoCauHoi { get; set; }

    public KyThi KyThi { get; set; } = null!;
    public ICollection<DeThiCauHoi> CauHois { get; set; } = new List<DeThiCauHoi>();
    public ICollection<BaiThi> BaiThis { get; set; } = new List<BaiThi>();
}