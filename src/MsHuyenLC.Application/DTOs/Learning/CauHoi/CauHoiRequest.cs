using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

namespace MsHuyenLC.Application.DTOs.Learning.CauHoi;

public class CauHoiRequest
{
    public string NoiDungCauHoi { get; set; } = null!;
    public KyNang KyNang { get; set; } = KyNang.Doc;
    public string? UrlHinhAnh { get; set; }
    public string? UrlAmThanh { get; set; }
    public string? LoiThoai { get; set; }
    public CapDo CapDo { get; set; } = CapDo.A1;
    public DoKho DoKho { get; set; } = DoKho.de;
    public ICollection<DapAnRequest>? CacDapAn { get; set; }
    public ICollection<NhomCauHoiChiTietRequest>? CacNhomCauHoiChiTiet { get; set; }
}