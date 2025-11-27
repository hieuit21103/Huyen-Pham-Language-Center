using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Domain.Entities.Learning;

public class PhanHoi
{
    public Guid Id { get; set; }
    
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string LoaiPhanHoi { get; set; } = null!;
    
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string TieuDe { get; set; } = null!;
    
    [MaxLength(2000)]
    [Column(TypeName = "varchar(2000)")]
    public string NoiDung { get; set; } = null!;
    
    public Guid HocVienId { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
