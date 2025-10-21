namespace MsHuyenLC.Domain.Entities.Courses;

public class PhongHoc
{
    public Guid Id { get; set; }
    public string TenPhong { get; set; } = null!;
    public int SoGhe { get; set; }
    public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}