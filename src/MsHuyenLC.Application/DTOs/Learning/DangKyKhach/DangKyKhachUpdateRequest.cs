using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKyKhach;

/// <summary>
/// DTO for Admin/GiaoVu to update guest registration details and process status
/// </summary>
public class DangKyKhachUpdateRequest
{
    public string? HoTen { get; set; }
    public GioiTinh GioiTinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public string? NoiDung { get; set; }
    public Guid? KhoaHocId { get; set; }
    public TrangThaiDangKy? TrangThai { get; set; }
    public KetQuaDangKy? KetQua { get; set; }
    public Guid? TaiKhoanXuLyId { get; set; }
}
