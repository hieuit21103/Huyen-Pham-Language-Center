using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class DapAnCauHoi
{
    public Guid Id { get; set; }
    public Guid CauHoiId { get; set; }
    
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string Nhan { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    [Column(TypeName = "varchar(1000)")]
    public string NoiDung { get; set; } = string.Empty;
    
    public bool Dung { get; set; }
    
    [MaxLength(1000)]
    [Column(TypeName = "varchar(1000)")]
    public string? GiaiThich { get; set; }
    
    public virtual CauHoi CauHoi { get; set; } = null!;
}

