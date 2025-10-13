namespace MsHuyenLC.Domain.Entities.Learning;

public class DangKy
{
    public Guid Id { get; set; }
    public DateTime NgayDangKy { get; set; } = DateTime.UtcNow;
    public TrangThaiDangKy TrangThai { get; set; } = TrangThaiDangKy.choduyet;

    public HocVien HocVien { get; set; } = null!;
    public LopHoc LopHoc { get; set; } = null!;
}





