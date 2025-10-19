namespace MsHuyenLC.Application.DTOs.Learning.DangKyKhach;

/// <summary>
/// DTO for Admin/GiaoVu to manually create guest registration after consultation
/// </summary>
public class DangKyKhachCreateDTO
{
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string? NoiDung { get; set; }
    public Guid KhoaHocId { get; set; }
}
