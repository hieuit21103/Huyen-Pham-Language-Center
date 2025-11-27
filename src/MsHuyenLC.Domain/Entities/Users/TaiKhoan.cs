using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Users;

public class TaiKhoan
{
    public Guid Id { get; set; }
    
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string TenDangNhap { get; set; } = null!;
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string MatKhau { get; set; } = null!;
    
    public VaiTro VaiTro { get; set; } = VaiTro.hocvien;
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    [Column(TypeName = "varchar(20)")]
    public string? Sdt { get; set; }
    
    public TrangThaiTaiKhoan TrangThai { get; set; } = TrangThaiTaiKhoan.hoatdong;
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? Avatar { get; set; }
    
    public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}




