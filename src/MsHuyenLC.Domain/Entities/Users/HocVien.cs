namespace MsHuyenLC.Domain.Entities.Users;

public class HocVien
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public DateTime? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public DateTime NgayDangKy { get; set; } = DateTime.Now;
    public TrangThaiHocVien TrangThai { get; set; } = TrangThaiHocVien.danghoc;

    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ICollection<DangKy> DangKys { get; set; } = new List<DangKy>();
    public ICollection<BaiThi> BaiThis { get; set; } = new List<BaiThi>();
    public ICollection<PhanHoi> PhanHois { get; set; } = new List<PhanHoi>();
    public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}