namespace MsHuyenLC.Domain.Entities.Learning;

public class DeThi
{
    public Guid Id { get; set; }
    public string TenDe { get; set; } = null!;
    public int SoCauHoi { get; set; }
    public Guid KyThiId { get; set; }
    public KyThi KyThi { get; set; } = null!;
}