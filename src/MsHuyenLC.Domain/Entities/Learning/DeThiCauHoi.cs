namespace MsHuyenLC.Domain.Entities.Learning;

public class DeThiCauHoi
{
    public Guid Id { get; set; }
    public DeThi DeThi { get; set; } = null!;
    public NganHangDe CauHoi { get; set; } = null!;
}