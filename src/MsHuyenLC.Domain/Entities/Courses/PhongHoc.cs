using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Courses;

public class PhongHoc
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string TenPhong { get; set; } = null!;
    
    public int SoGhe { get; set; }
    public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
