namespace MsHuyenLC.Domain.Entities.Courses;

public class PhongHoc
{
    public Guid Id { get; set; }
    public string TenPhong { get; set; } = null!;
    public int SoGhe { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
}