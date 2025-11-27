using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.DTOs.Courses.LopHoc;

public class DanhSachLopResponse
{
    public string Id { get; set; } = null!;
    public string TenLop { get; set; } = null!;
    public List<HocVienLopResponse> DanhSachHocVien { get; set; } = new List<HocVienLopResponse>();
}