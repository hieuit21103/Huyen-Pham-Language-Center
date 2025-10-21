using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Domain.Entities.Learning;

public class ThongBao
{
    public Guid Id { get; set; }
    public Guid NguoiGui { get; set; }
    public DoiTuongNhan DoiTuongNhan { get; set; } = DoiTuongNhan.lophoc;
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public DateOnly NgayGui { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public Guid? LopHocId { get; set; }
    public TaiKhoan NguoiGuiTaiKhoan { get; set; } = null!;
    public LopHoc? LopHoc { get; set; }
}