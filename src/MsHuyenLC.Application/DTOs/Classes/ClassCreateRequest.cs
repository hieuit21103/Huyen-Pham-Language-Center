using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Classes;

public class ClassCreateRequest
{
    public string TenLop { get; set; } = null!;
    public string? PhongHoc { get; set; }
    public int SiSoToiDa { get; set; }
    public TrangThaiLopHoc TrangThai { get; set; } = TrangThaiLopHoc.choxepgiaovien;
    public string KhoaHocId { get; set; } = null!;
}