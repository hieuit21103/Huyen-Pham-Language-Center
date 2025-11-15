namespace MsHuyenLC.Application.DTOs.Courses.LichHoc;

public class LichHocUpdateRequest
{    public Guid PhongHocId { get; set; }
    public DayOfWeek Thu { get; set; }
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public DateOnly TuNgay { get; set; }
    public DateOnly DenNgay { get; set; }
    public bool CoHieuLuc { get; set; }
}
