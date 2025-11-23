using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class CauTraLoi
{
    public Guid Id { get; set; }
    public Guid PhienId { get; set; }
    public Guid CauHoiId { get; set; }
    
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string? CauTraLoiText { get; set; }
    
    public bool? Dung { get; set; }
    public virtual PhienLamBai Phien { get; set; } = null!;
    public virtual CauHoi CauHoi { get; set; } = null!;
}
