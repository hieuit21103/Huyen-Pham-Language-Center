namespace MsHuyenLC.Domain.Entities.Users;

public class TaiKhoan
{
    public Guid Id { get; set; }
    public string TenDangNhap { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
    public VaiTro VaiTro { get; set; } = VaiTro.hocvien;
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public TrangThaiTaiKhoan TrangThai { get; set; } = TrangThaiTaiKhoan.hoatdong;
    public string? Avatar { get; set; }
    public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}




