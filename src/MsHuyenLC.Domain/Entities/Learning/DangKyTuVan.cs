namespace MsHuyenLC.Domain.Entities.Learning;
public class DangKyTuVan
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public GioiTinh GioiTinh { get; set; }
    public string Email { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string? NoiDung { get; set; }
    public DateOnly NgayDangKy { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TrangThaiDangKy TrangThai { get; set; } = TrangThaiDangKy.choduyet;
    public KetQuaDangKy KetQua { get; set; } = KetQuaDangKy.chuaxuly;
    public DateOnly? NgayXuLy { get; set; }
    public Guid KhoaHocId { get; set; }
    public Guid? TaiKhoanXuLyId { get; set; }
    public KhoaHoc KhoaHoc { get; set; } = null!;
    public TaiKhoan? TaiKhoanXuLy { get; set; }
}