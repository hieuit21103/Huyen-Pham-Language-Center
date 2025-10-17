namespace MsHuyenLC.Application.DTOs.Courses;

public class ScheduleUpdateRequest
{
    public string? LopHocId { get; set; }
    public string? PhongHocId { get; set; }
    public DayOfWeek? Thu { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public TimeSpan? GioBatDau { get; set; }
    public TimeSpan? GioKetThuc { get; set; }   
    public bool? CoHieuLuc { get; set; }
}