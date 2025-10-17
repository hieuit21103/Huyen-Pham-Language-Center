using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Courses.LopHoc;

public class LopHocRequest
{
    public string TenLop { get; set; } = null!;
    public Guid KhoaHocId { get; set; }
    public int SiSoToiDa { get; set; }
}
