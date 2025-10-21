namespace MsHuyenLC.Domain.Entities.Courses;

public class KhoaHoc
{
    public Guid Id { get; set; }
    public string TenKhoaHoc { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal HocPhi { get; set; }
    public int ThoiLuong { get; set; }
    public DateOnly NgayKhaiGiang { get; set; }
    public TrangThaiKhoaHoc TrangThai { get; set; } = TrangThaiKhoaHoc.dangmo;
}

