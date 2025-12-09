using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

public class NhomCauHoiUpdateRequest
{
    public string? UrlAmThanh { get; set; }
    public string? UrlHinhAnh { get; set; }
    public string? NoiDung { get; set; }
    public string? TieuDe { get; set; }
    public int? SoLuongCauHoi { get; set; }
    public DoKho? DoKho { get; set; }
    public CapDo? CapDo { get; set; }
    public KyNang? KyNang { get; set; }
}

