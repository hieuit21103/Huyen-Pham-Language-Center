using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Courses.LopHoc;

public class HocVienLopResponse
{
    public string Id { get; set; } = null!;
    public string HoTen { get; set; } = null!;
    public GioiTinh GioiTinh { get; set; }
    public string Email { get; set; } = null!;
    public string Sdt { get; set; } = null!;
}