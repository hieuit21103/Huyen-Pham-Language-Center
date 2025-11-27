using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Users;

public class GiaoVu
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string HoTen { get; set; } = null!;
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string? BoPhan { get; set; }
    
    public Guid TaiKhoanId { get; set; }
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
