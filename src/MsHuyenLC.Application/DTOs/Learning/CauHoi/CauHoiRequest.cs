using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

namespace MsHuyenLC.Application.DTOs.Learning.CauHoi;

public class CauHoiRequest
{
    public string NoiDungCauHoi { get; set; } = null!;
    public LoaiCauHoi LoaiCauHoi { get; set; } = LoaiCauHoi.TracNghiem;
    public KyNang KyNang { get; set; } = KyNang.Doc;
    public string? UrlHinhAnh { get; set; }
    public string? UrlAmThanh { get; set; }
    public string? LoiThoai { get; set; }
    public CapDo CapDo { get; set; } = CapDo.A1;
    public DoKho DoKho { get; set; } = DoKho.de;
    public ICollection<DapAnRequest>? DapAnCauHois { get; set; }
    public ICollection<NhomCauHoiChiTietRequest>? NhomCauHoiChiTiets { get; set; }
}