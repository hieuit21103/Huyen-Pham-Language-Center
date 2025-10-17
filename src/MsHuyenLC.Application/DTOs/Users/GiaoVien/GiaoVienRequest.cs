namespace MsHuyenLC.Application.DTOs.Users.GiaoVien;

public class GiaoVienRequest
{
    public string HoTen { get; set; } = null!;
    public string? ChuyenMon { get; set; }
    public string? TrinhDo { get; set; }
    public string? KinhNghiem { get; set; }
    public Guid TaiKhoanId { get; set; }
}
