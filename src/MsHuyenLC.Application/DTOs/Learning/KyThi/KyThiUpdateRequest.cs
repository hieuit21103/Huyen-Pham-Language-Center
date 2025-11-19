using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.KyThi;

public class KyThiUpdateRequest
{
    public string? TenKyThi { get; set; }
    public DateOnly? NgayThi { get; set; }
    public TimeOnly? GioBatDau { get; set; }
    public TimeOnly? GioKetThuc { get; set; }
    public int? ThoiLuong { get; set; }
    public Guid? LopHocId { get; set; }
    public TrangThaiKyThi? TrangThai { get; set; }
    public ICollection<CauHinhKyThiUpdateRequest>? CauHinhKyThis { get; set; }
}
