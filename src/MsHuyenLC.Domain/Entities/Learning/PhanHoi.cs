using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Domain.Entities.Learning;

public class PhanHoi
{
    public Guid Id { get; set; }
    public string NoiDung { get; set; } = null!;
    public DateOnly NgayGui { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public Guid HocVienId { get; set; }
    public Guid GiaoVienId { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public GiaoVien GiaoVien { get; set; } = null!;
}