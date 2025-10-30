using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Domain.Entities.Learning;

public class PhanHoi
{
    public Guid Id { get; set; }
    public string LoaiPhanHoi { get; set; } = null!;
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public Guid HocVienId { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public DateOnly NgayTao { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}