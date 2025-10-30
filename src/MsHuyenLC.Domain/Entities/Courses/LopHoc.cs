namespace MsHuyenLC.Domain.Entities.Courses;

public class LopHoc
{
    public Guid Id { get; set; }
    public string TenLop { get; set; } = null!;
    public int SiSoHienTai { get; set; }
    public int SiSoToiDa { get; set; }
    public TrangThaiLopHoc TrangThai { get; set; } = TrangThaiLopHoc.danghoc;
    public Guid KhoaHocId { get; set; }
    public KhoaHoc KhoaHoc { get; set; } = null!;
    public ICollection<DangKy> CacDangKy { get; set; } = new List<DangKy>();
    public void CapNhatSiSo(int soLuongThayDoi)
    {
        SiSoHienTai += soLuongThayDoi;
        if (SiSoHienTai < 0)
        {
            SiSoHienTai = 0;
        }
    }
}