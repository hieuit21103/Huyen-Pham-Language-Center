namespace MsHuyenLC.Domain.Entities.Learning;

public class BaiThiChiTiet
{
    public Guid Id { get; set; }
    public string? CauTraLoi { get; set; }
    public float? Diem { get; set; }
    public string? NhanXet { get; set; }
    public Guid BaiThiId { get; set; }
    public Guid CauHoiId { get; set; }
    public BaiThi BaiThi { get; set; } = null!;
    public CauHoi CauHoi { get; set; } = null!;
}