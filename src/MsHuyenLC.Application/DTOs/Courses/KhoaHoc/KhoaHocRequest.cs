namespace MsHuyenLC.Application.DTOs.Courses.KhoaHoc;

public class KhoaHocRequest
{
    public string TenKhoaHoc { get; set; } = null!;
    public string? MoTa { get; set; }
    public decimal HocPhi { get; set; }
    public int ThoiLuong { get; set; }
    public DateTime NgayKhaiGiang { get; set; }
}
