using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.KyThi;

public class KyThiRequest
{
    public string TenKyThi { get; set; } = null!;
    public DateOnly NgayThi { get; set; }
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public int ThoiLuong { get; set; }
    public Guid LopHocId { get; set; }
}
