using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Domain.Entities.Learning;

public class KyThi
{
    public Guid Id { get; set; }
    public string TenKyThi { get; set; } = null!;
    public DateOnly NgayThi { get; set; }
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public int ThoiLuong { get; set; }
    public TrangThaiKyThi TrangThai { get; set; } = TrangThaiKyThi.sapdienra;
    public Guid LopHocId { get; set; }
    public LopHoc LopHoc { get; set; } = null!;
}