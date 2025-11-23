using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.System;

public class NhatKyHeThong
{
    public Guid Id { get; set; }
    public DateOnly ThoiGian { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string HanhDong { get; set; } = null!;
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? ChiTiet { get; set; }
    
    [Column(TypeName = "text")]
    public string DuLieuCu { get; set; } = null!;
    
    [Column(TypeName = "text")]
    public string DuLieuMoi { get; set; } = null!;
    
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string IP { get; set; } = null!;
    
    public Guid TaiKhoanId { get; set; }
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
