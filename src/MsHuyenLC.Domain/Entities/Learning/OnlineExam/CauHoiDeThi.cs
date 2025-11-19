namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class CauHoiDeThi
{
    public Guid Id { get; set; }
    public Guid DeThiId { get; set; }
    public Guid CauHoiId { get; set; }
    public int ThuTuCauHoi { get; set; }
    public decimal Diem { get; set; } = 1;
    public Guid? NhomCauHoiId { get; set; }
    public virtual DeThi DeThi { get; set; } = null!;
    public virtual CauHoi CauHoi { get; set; } = null!;
    public virtual NhomCauHoi? NhomCauHoi { get; set; }
}