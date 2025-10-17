namespace MsHuyenLC.Application.DTOs.Users.GiaoVu;

public class GiaoVuRequest
{
    public string HoTen { get; set; } = null!;
    public string? BoPhan { get; set; }
    public Guid TaiKhoanId { get; set; }
}
