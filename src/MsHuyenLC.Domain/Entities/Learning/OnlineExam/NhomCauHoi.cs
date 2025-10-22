namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class NhomCauHoi
{
    public Guid Id { get; set; }
    public string? UrlAmThanh { get; set; }
    public string? NoiDung { get; set; }
    public string? TieuDe { get; set; }
    public int SoLuongCauHoi { get; set; }
    public DoKho DoKho { get; set; } = DoKho.trungbinh;
    public CapDo CapDo { get; set; } = CapDo.A1;
    public virtual ICollection<NhomCauHoiChiTiet> CacChiTiet { get; set; } = new List<NhomCauHoiChiTiet>();
    public virtual ICollection<CauHoiDeThi> CacCauHoiDeThi { get; set; } = new List<CauHoiDeThi>();
}

