namespace MsHuyenLC.Domain.Entities.Courses;

public class LichHoc
{
    public Guid Id { get; set; }
    public DayOfWeek Thu { get; set; } 
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public DateOnly TuNgay { get; set; }
    public DateOnly DenNgay { get; set; }
    public bool CoHieuLuc { get; set; } = true;
    public Guid LopHocId { get; set; }
    public Guid PhongHocId { get; set; }
    public LopHoc LopHoc { get; set; } = null!;
    public PhongHoc PhongHoc { get; set; } = null!;
}