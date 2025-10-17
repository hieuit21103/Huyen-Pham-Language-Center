namespace MsHuyenLC.Domain.Entities.Courses;

public class LichHoc
{
    public Guid Id { get; set; }
    public DayOfWeek Thu { get; set; } 
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public DateTime TuNgay { get; set; }
    public DateTime DenNgay { get; set; }
    public bool CoHieuLuc { get; set; } = true;
    public Guid LopHocId { get; set; }
    public Guid PhongHocId { get; set; }
    public LopHoc LopHoc { get; set; } = null!;
    public PhongHoc PhongHoc { get; set; } = null!;
}