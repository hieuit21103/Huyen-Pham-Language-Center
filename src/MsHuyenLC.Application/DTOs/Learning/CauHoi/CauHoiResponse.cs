using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;

namespace MsHuyenLC.Application.DTOs.Learning.CauHoi;

public class CauHoiResponse
{
    public string NoiDungCauHoi { get; set; } = null!;
    public KyNang KyNang { get; set; } = KyNang.Doc;
    public string? UrlHinhAnh { get; set; }
    public string? UrlAmThanh { get; set; }
    public string? LoiThoai { get; set; }
    public CapDo CapDo { get; set; } = CapDo.A1;
    public DoKho DoKho { get; set; } = DoKho.de;
    public ICollection<DapAnResponse>? DapAnCauHois { get; set; }
}