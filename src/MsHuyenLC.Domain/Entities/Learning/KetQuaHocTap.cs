using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Domain.Entities.Learning;

public class KetQuaHocTap
{
    public Guid Id { get; set; }
    public decimal DiemSo { get; set; }
    public DateTime NgayThi { get; set; } = DateTime.Now;

    public HocVien HocVien { get; set; } = null!;
    public KyThi KyThi { get; set; } = null!;
}