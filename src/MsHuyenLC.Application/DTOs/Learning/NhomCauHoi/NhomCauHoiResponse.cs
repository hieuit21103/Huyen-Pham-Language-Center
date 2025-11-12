using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

public class NhomCauHoiResponse
{
    public string Id { get; set; } = null!;
    public string? UrlAmThanh { get; set; }
    public string? UrlHinhAnh { get; set; }
    public string? NoiDung { get; set; }
    public string TieuDe { get; set; } = null!;
    public int SoLuongCauHoi { get; set; }
    public DoKho DoKho { get; set; }
    public CapDo CapDo { get; set; }
}
