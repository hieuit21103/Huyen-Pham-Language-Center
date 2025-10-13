namespace MsHuyenLC.Domain.Entities.Learning;

public class BaiThi
{
    public Guid Id { get; set; }
    public float? DiemTracNghiem { get; set; }
    public float? DiemTuLuan { get; set; }
    public float? TongDiem { get; set; }
    public string? NhanXet { get; set; }
    public DateTime NgayNop { get; set; } = DateTime.UtcNow;

    public DeThi DeThi { get; set; } = null!;
    public HocVien HocVien { get; set; } = null!;
    public ICollection<BaiThiChiTiet> BaiThiChiTiets { get; set; } = new List<BaiThiChiTiet>();
}