namespace MsHuyenLC.Domain.Entities.Learning;

public class DangKy
{
    public Guid Id { get; set; }
    public DateOnly NgayDangKy { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TrangThaiDangKy TrangThai { get; set; } = TrangThaiDangKy.choduyet;
    public Guid HocVienId { get; set; }
    public Guid KhoaHocId { get; set; }
    public Guid? LopHocId { get; set; }
    public DateOnly? NgayXepLop { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public KhoaHoc KhoaHoc { get; set; } = null!;
    public LopHoc? LopHoc { get; set; }
}





