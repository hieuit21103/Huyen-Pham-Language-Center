namespace MsHuyenLC.Application.DTOs.Teachers;

public class TeacherResponse
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public string? ChuyenMon { get; set; }
    public string? TrinhDo { get; set; }
    public string? KinhNghiem { get; set; }
    public string Email { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public int SoLopDangDay { get; set; }
}
