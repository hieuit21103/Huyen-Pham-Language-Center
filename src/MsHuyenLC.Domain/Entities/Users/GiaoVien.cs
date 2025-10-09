namespace MsHuyenLC.Domain.Entities.Users;

public class GiaoVien
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public string? ChuyenMon { get; set; }
    public string? TrinhDo { get; set; }
    public string? KinhNghiem { get; set; }

    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ICollection<PhanCong> PhanCongs { get; set; } = new List<PhanCong>();
    public ICollection<PhanHoi> PhanHois { get; set; } = new List<PhanHoi>();
}
