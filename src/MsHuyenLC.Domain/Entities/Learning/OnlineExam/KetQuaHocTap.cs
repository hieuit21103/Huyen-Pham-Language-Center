using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Domain.Entities.Learning;

public class KetQuaHocTap
{
    public Guid Id { get; set; }
    public decimal DiemSo { get; set; }
    public DateOnly NgayThi { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public Guid HocVienId { get; set; }
    public Guid KyThiId { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public KyThi KyThi { get; set; } = null!;
}