using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

public class NhomCauHoiRequest
{
    public string? UrlAmThanh { get; set; }
    public string? UrlHinhAnh { get; set; }
    public string? NoiDung { get; set; }
    public string TieuDe { get; set; } = null!;
    public int SoLuongCauHoi { get; set; }
    public DoKho DoKho { get; set; } = DoKho.trungbinh;
    public CapDo CapDo { get; set; } = CapDo.A1;
}

