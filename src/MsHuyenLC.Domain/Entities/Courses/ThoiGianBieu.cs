namespace MsHuyenLC.Domain.Entities.Courses;

public class ThoiGianBieu
{
    public Guid Id { get; set; }
    public Guid LichHocId { get; set; }
    public LichHoc LichHoc { get; set; } = null!;
    public DayOfWeek Thu { get; set; } 
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
}