namespace MsHuyenLC.Domain.Entities.Learning;

public class DangKyKhach
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string? NoiDung { get; set; }
    public DateTime NgayDangKy { get; set; } = DateTime.Now;
    public TrangThaiDangKy TrangThai { get; set; } = TrangThaiDangKy.choduyet;
    public KetQuaDangKy KetQua { get; set; } = KetQuaDangKy.chuaxuly;
    public DateTime? NgayXuLy { get; set; }
    public KhoaHoc KhoaHoc { get; set; } = null!;
    public TaiKhoan? TaiKhoanXuLy { get; set; }
}