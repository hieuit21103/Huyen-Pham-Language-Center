namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class DeThi
{
    public Guid Id { get; set; }
    public string MaDe { get; set; } = string.Empty;
    public string TenDe { get; set; } = string.Empty;
    public LoaiDeThi LoaiDeThi { get; set; } = LoaiDeThi.LuyenTap;
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    public Guid NguoiTaoId { get; set; }
    public int TongCauHoi { get; set; }
    public int ThoiLuongPhut { get; set; }
    public Guid? KyThiId { get; set; }
    public virtual KyThi? KyThi { get; set; }
    public virtual TaiKhoan? NguoiTao { get; set; }
    public virtual ICollection<CauHoiDeThi> CacCauHoi { get; set; } = new List<CauHoiDeThi>();
}
      