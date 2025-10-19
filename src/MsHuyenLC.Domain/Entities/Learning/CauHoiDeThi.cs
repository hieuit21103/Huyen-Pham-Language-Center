namespace MsHuyenLC.Domain.Entities.Learning;

public class CauHoiDeThi
{
    public Guid Id { get; set; }
    public Guid DeThiId { get; set; }
    public Guid CauHoiId { get; set; }
    public DeThi DeThi { get; set; } = null!;
    public CauHoi CauHoi { get; set; } = null!;
}