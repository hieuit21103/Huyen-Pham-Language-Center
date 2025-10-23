using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKyKhach;

public class DangKyKhachResponse
{
    public Guid Id { get; set; }
    public string HoTen { get; set; } = null!;
    public GioiTinh GioiTinh { get; set; }
    public string Email { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public string? NoiDung { get; set; }
    public DateOnly NgayDangKy { get; set; }
    public TrangThaiDangKy TrangThai { get; set; }
    public KetQuaDangKy KetQua { get; set; }
    public DateOnly? NgayXuLy { get; set; }
    
    // Related data
    public Guid KhoaHocId { get; set; }
    public string? TenKhoaHoc { get; set; }
    public Guid? TaiKhoanXuLyId { get; set; }
    public string? NguoiXuLy { get; set; }
}
