using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Courses;

public class KhoaHoc
{
    public Guid Id { get; set; }
    
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string TenKhoaHoc { get; set; } = null!;
    
    [MaxLength(1000)]
    [Column(TypeName = "varchar(1000)")]
    public string? MoTa { get; set; }
    
    public decimal HocPhi { get; set; }
    public int ThoiLuong { get; set; }
    public DateOnly NgayKhaiGiang { get; set; }
    public TrangThaiKhoaHoc TrangThai { get; set; } = TrangThaiKhoaHoc.dangmo;
}

