namespace MsHuyenLC.Application.DTOs.Learning.DangKyKhach;

public class DangKyKhachRequest
{
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string? NoiDung { get; set; }
    public Guid KhoaHocId { get; set; }
}
