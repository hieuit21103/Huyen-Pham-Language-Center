using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Users;

public class GiaoVien
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string HoTen { get; set; } = null!;
    
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? ChuyenMon { get; set; }
    
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? TrinhDo { get; set; }
    
    [MaxLength(1000)]
    [Column(TypeName = "varchar(1000)")]
    public string? KinhNghiem { get; set; }
    
    public Guid TaiKhoanId { get; set; }
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
