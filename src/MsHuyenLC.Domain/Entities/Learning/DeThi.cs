namespace MsHuyenLC.Domain.Entities.Learning;

public class DeThi
{
    public Guid Id { get; set; }
    public string TenDe { get; set; } = null!;
    public int SoCauHoi { get; set; }
    public LoaiDeThi LoaiDeThi { get; set; } = LoaiDeThi.LuyenTap;
    public Guid? KyThiId { get; set; }
    public KyThi? KyThi { get; set; }
    public int ThoiGianLamBai { get; set; } // in minutes
    public ICollection<CauHoiDeThi> CauHoiDeThis { get; set; } = new List<CauHoiDeThi>();
}