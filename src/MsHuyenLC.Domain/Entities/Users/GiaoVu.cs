namespace MsHuyenLC.Domain.Entities.Users;

public class GiaoVu
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public string? BoPhan { get; set; }

    public TaiKhoan TaiKhoan { get; set; } = null!;
}
