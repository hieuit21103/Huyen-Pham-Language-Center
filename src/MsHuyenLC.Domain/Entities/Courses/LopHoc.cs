namespace MsHuyenLC.Domain.Entities.Courses;

public class LopHoc
{
    public Guid Id { get; set; }
    public string TenLop { get; set; } = null!;
    public string? PhongHoc { get; set; }
    public int SiSoHienTai { get; set; }
    public int SiSoToiDa { get; set; }
    public TrangThaiLopHoc TrangThai { get; set; } = TrangThaiLopHoc.danghoc;

    public KhoaHoc KhoaHoc { get; set; } = null!;
    public ICollection<PhanCong> PhanCongs { get; set; } = new List<PhanCong>();
    public ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();
    public ICollection<DangKy> DangKys { get; set; } = new List<DangKy>();
    public ICollection<KyThi> KyThis { get; set; } = new List<KyThi>();
    public ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();

    public void ThemHocSinh(DangKy dangKy)
    {
        if (SiSoHienTai < SiSoToiDa)
        {
            DangKys.Add(dangKy);
            SiSoHienTai = DangKys.Count;
        }
        else
        {
            throw new InvalidOperationException("Lớp đã đủ sĩ số tối đa.");
        }
    }
}