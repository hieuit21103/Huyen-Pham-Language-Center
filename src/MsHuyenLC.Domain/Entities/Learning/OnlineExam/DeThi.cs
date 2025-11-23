using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class DeThi
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string MaDe { get; set; } = string.Empty;
    
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string TenDe { get; set; } = string.Empty;    
    
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    public Guid NguoiTaoId { get; set; }
    public int ThoiLuongPhut { get; set; }
    public Guid? KyThiId { get; set; }
    public virtual KyThi? KyThi { get; set; }
    public virtual TaiKhoan? NguoiTao { get; set; }
    public virtual ICollection<CauHoiDeThi> CacCauHoi { get; set; } = new List<CauHoiDeThi>();
}
      
