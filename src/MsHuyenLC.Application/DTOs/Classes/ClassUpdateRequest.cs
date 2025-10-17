using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Classes;

public class ClassUpdateRequest
{
    public string? TenLop { get; set; }
    public int? SiSoToiDa { get; set; }
    public TrangThaiLopHoc? TrangThai { get; set; }
    public string? KhoaHocId { get; set; }
}