namespace MsHuyenLC.Application.DTOs.Courses;

public class ScheduleCreateRequest
{
    public string LopHocId { get; set; } = null!;
    public string PhongHocId { get; set; } = null!;
    public DayOfWeek Thu { get; set; }
    public DateTime TuNgay { get; set; }
    public DateTime DenNgay { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }   
}