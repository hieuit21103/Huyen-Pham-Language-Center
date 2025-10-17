namespace MsHuyenLC.Application.DTOs.Courses.LichHoc;

public class LichHocRequest
{
    public Guid LopHocId { get; set; }
    public Guid PhongHocId { get; set; }
    public DayOfWeek Thu { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public DateTime TuNgay { get; set; }
    public DateTime DenNgay { get; set; }
}
