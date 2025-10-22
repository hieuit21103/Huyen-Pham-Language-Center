using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.CauHoi;

public class CauHoiUpdateRequest
{
    public string? NoiDungCauHoi { get; set; }
    public LoaiCauHoi? LoaiCauHoi { get; set; }
    public KyNang? KyNang { get; set; }
    public string? UrlHinhAnh { get; set; }
    public string? UrlAmThanh { get; set; }
    public string? LoiThoai { get; set; }
    public string? DoanVan { get; set; }
    public CapDo? CapDo { get; set; }
    public DoKho? DoKho { get; set; }
}