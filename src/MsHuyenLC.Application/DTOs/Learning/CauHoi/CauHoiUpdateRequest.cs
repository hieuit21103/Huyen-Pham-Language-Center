using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.DTOs.Learning.CauHoi;

public class CauHoiUpdateRequest
{
    public string? NoiDung { get; set; }
    public LoaiCauHoi? LoaiCauHoi { get; set; } 
    public KyNang? KyNang { get; set; }
    public string? UrlHinh { get; set; }
    public string? UrlAmThanh { get; set; }
    public string? DapAnDung { get; set; }
    public string? GiaiThich { get; set; }
    public CapDo? CapDo { get; set; }
    public DoKho? DoKho { get; set; }
    public Guid? DocHieuId { get; set; }
    public DocHieu? DocHieu { get; set; }
}