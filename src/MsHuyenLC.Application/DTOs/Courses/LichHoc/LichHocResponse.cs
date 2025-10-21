namespace MsHuyenLC.Application.DTOs.Courses.LichHoc;

public class LichHocResponse
{
    public Guid Id { get; set; }
    public Guid LopHocId { get; set; }
    public string? TenLop { get; set; }
    public Guid PhongHocId { get; set; }
    public string? TenPhong { get; set; }
    public DayOfWeek Thu { get; set; }
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public DateOnly TuNgay { get; set; }
    public DateOnly DenNgay { get; set; }
    public bool CoHieuLuc { get; set; }
}
