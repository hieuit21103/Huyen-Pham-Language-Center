using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Domain.Entities.Learning;

public class PhanHoi
{
    public Guid Id { get; set; }
    public string NoiDung { get; set; } = null!;
    public DateTime NgayGui { get; set; } = DateTime.UtcNow;

    public HocVien HocVien { get; set; } = null!;
    public GiaoVien GiaoVien { get; set; } = null!;
}