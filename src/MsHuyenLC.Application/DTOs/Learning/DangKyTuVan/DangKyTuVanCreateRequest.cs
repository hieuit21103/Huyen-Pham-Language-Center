using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKyTuVan;

/// <summary>
/// DTO for Admin/GiaoVu to manually create guest registration after consultation
/// </summary>
public class DangKyTuVanCreateRequest
{
    public string HoTen { get; set; } = null!;
    public GioiTinh GioiTinh { get; set; }
    public string Email { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string? NoiDung { get; set; }
    public Guid KhoaHocId { get; set; }
}
