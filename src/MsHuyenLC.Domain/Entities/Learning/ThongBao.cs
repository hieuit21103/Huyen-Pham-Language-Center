using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Learning;

public class ThongBao
{
    public Guid Id { get; set; }
    
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string TieuDe { get; set; } = null!;
    
    [MaxLength(2000)]
    [Column(TypeName = "varchar(2000)")]
    public string NoiDung { get; set; } = null!;
    
    public Guid NguoiGuiId { get; set; }
    public TaiKhoan NguoiGui { get; set; } = null!;
    public Guid? NguoiNhanId { get; set; }
    public TaiKhoan NguoiNhan { get; set; } = null!;
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
}
