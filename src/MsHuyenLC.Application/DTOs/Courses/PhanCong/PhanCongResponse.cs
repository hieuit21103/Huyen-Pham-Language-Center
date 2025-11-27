using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.DTOs.Courses.PhanCong;

public class PhanCongResponse
{
    public Guid Id { get; set; }
    public Guid GiaoVienId { get; set; }
    public GiaoVien? GiaoVien { get; set; }
    public Guid LopHocId { get; set; }
    public Domain.Entities.Courses.LopHoc? LopHoc { get; set; }
    public DateOnly NgayPhanCong { get; set; }
}
