using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class CauHoi
{
    public Guid Id { get; set; }
    
    [MaxLength(2000)]
    [Column(TypeName = "varchar(2000)")]
    public string NoiDungCauHoi { get; set; } = string.Empty;
    
    public KyNang KyNang { get; set; } = KyNang.Doc;
    public DoKho DoKho { get; set; } = DoKho.trungbinh;
    public CapDo CapDo { get; set; } = CapDo.A1;
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? UrlAmThanh { get; set; }
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? UrlHinhAnh { get; set; }
    
    [Column(TypeName = "text")]
    public string? LoiThoai { get; set; }
    
    public virtual ICollection<DapAnCauHoi> CacDapAn { get; set; } = new List<DapAnCauHoi>();
    public virtual ICollection<NhomCauHoiChiTiet> CacNhom { get; set; } = new List<NhomCauHoiChiTiet>();
}
