namespace MsHuyenLC.Domain.Entities.Courses;

public class LichHoc
{
    public Guid Id { get; set; }
    public DateTime NgayHoc { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public string? PhongHoc { get; set; }
    public LopHoc LopHoc { get; set; } = null!;
}