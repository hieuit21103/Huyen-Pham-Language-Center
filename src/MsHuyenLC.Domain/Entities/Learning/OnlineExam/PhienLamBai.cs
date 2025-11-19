namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class PhienLamBai
{
    public Guid Id { get; set; }
    public int TongCauHoi { get; set; }
    public int? SoCauDung { get; set; }
    public decimal? Diem { get; set; }
    public TimeSpan ThoiGianLam { get; set; }
    public DateOnly NgayLam { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public Guid HocVienId { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public Guid DeThiId { get; set; }
    public DeThi DeThi { get; set; } = null!;
    public Guid? KyThiId { get; set; }
    public KyThi? KyThi { get; set; }
}