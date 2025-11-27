using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Learning;
public class DangKyTuVan
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string HoTen { get; set; } = null!;
    
    public GioiTinh GioiTinh { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string Email { get; set; } = null!;
    
    [MaxLength(20)]
    [Column(TypeName = "varchar(20)")]
    public string SoDienThoai { get; set; } = null!;
    
    [MaxLength(1000)]
    [Column(TypeName = "varchar(1000)")]
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
