namespace MsHuyenLC.Domain.Entities.Learning;

public class BaiThiChiTiet
{
    public Guid Id { get; set; }
    public BaiThi BaiThi { get; set; } = null!;
    public DeThiCauHoi CauHoi { get; set; } = null!;
    public string? CauTraLoi { get; set; }
    public float? Diem { get; set; }
    public string? NhanXet { get; set; }
}