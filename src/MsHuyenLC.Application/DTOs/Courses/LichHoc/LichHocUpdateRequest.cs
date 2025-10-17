namespace MsHuyenLC.Application.DTOs.Courses.LichHoc;

public class LichHocUpdateRequest
{
    public Guid PhongHocId { get; set; }
    public DayOfWeek Thu { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public DateTime TuNgay { get; set; }
    public DateTime DenNgay { get; set; }
    public bool CoHieuLuc { get; set; }
}
