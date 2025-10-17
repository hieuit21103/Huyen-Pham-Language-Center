namespace MsHuyenLC.Domain.Entities.Users;

public class HocVien
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public DateTime? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public DateTime NgayDangKy { get; set; } = DateTime.UtcNow;
    public TrangThaiHocVien TrangThai { get; set; } = TrangThaiHocVien.danghoc;
    public Guid TaiKhoanId { get; set; }
    public TaiKhoan TaiKhoan { get; set; } = null!;
}