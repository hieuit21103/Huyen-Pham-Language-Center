namespace MsHuyenLC.Application.DTOs.Courses.LichHoc;

public class LichHocResponse
{
    public Guid Id { get; set; }
    public Guid LopHocId { get; set; }
    public string? TenLop { get; set; }
    public Guid PhongHocId { get; set; }
    public string? TenPhong { get; set; }
    public DayOfWeek Thu { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public DateTime TuNgay { get; set; }
    public DateTime DenNgay { get; set; }
    public bool CoHieuLuc { get; set; }
}
