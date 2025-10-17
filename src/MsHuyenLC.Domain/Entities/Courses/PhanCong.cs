namespace MsHuyenLC.Domain.Entities.Courses;

public class PhanCong
{
    public Guid Id { get; set; }
    public DateTime NgayPhanCong { get; set; } = DateTime.UtcNow;
    public Guid LopHocId { get; set; }
    public Guid GiaoVienId { get; set; }
    public LopHoc LopHoc { get; set; } = null!;
    public GiaoVien GiaoVien { get; set; } = null!;
}