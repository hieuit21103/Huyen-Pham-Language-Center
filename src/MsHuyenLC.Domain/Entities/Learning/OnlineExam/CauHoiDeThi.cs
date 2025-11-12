using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Domain.Entities.Learning;

public class CauHoiDeThi
{
    public Guid Id { get; set; }
    public Guid DeThiId { get; set; }
    public Guid CauHoiId { get; set; }
    public int ThuTuCauHoi { get; set; }
    public decimal Diem { get; set; } = 1;
    public virtual DeThi DeThi { get; set; } = null!;
    public virtual CauHoi CauHoi { get; set; } = null!;
}