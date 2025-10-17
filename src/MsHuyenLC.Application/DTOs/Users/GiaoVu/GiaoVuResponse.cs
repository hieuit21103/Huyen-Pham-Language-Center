namespace MsHuyenLC.Application.DTOs.Users.GiaoVu;

public class GiaoVuResponse
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public string? BoPhan { get; set; }
    public Guid TaiKhoanId { get; set; }
    public string? Email { get; set; }
    public string? Sdt { get; set; }
}
