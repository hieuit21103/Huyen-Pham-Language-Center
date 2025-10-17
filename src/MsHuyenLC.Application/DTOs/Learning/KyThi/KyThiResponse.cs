using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.KyThi;

public class KyThiResponse
{
    public Guid Id { get; set; }
    public string TenKyThi { get; set; } = null!;
    public DateTime NgayThi { get; set; }
    public int ThoiLuong { get; set; }
    public Guid LopHocId { get; set; }
    public string? TenLop { get; set; }
    public HinhThucKyThi HinhThuc { get; set; }
    public TrangThaiKyThi TrangThai { get; set; }
}
