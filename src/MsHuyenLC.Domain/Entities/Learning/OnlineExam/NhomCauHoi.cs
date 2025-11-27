using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class NhomCauHoi
{
    public Guid Id { get; set; }
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? UrlAmThanh { get; set; }
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? UrlHinhAnh { get; set; }
    
    [Column(TypeName = "text")]
    public string? NoiDung { get; set; }
    
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string TieuDe { get; set; } = string.Empty;
    
    public int SoLuongCauHoi { get; set; }
    public DoKho DoKho { get; set; } = DoKho.trungbinh;
    public CapDo CapDo { get; set; } = CapDo.A1;
    public KyNang KyNang { get; set; } = KyNang.Doc;
    public virtual ICollection<NhomCauHoiChiTiet> CacChiTiet { get; set; } = new List<NhomCauHoiChiTiet>();
    public virtual ICollection<CauHoiDeThi> CacCauHoiDeThi { get; set; } = new List<CauHoiDeThi>();
}

