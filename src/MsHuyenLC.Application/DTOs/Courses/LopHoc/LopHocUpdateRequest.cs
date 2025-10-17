using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Courses.LopHoc;

public class LopHocUpdateRequest
{
    public string TenLop { get; set; } = null!;
    public int SiSoToiDa { get; set; }
    public TrangThaiLopHoc TrangThai { get; set; }
}
