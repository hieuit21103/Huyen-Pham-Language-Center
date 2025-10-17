namespace MsHuyenLC.Application.DTOs.Learning.BaiThi;

public class BaiThiResponse
{
    public Guid Id { get; set; }
    public Guid DeThiId { get; set; }
    public string? TenDeThi { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public float? DiemTracNghiem { get; set; }
    public float? DiemTuLuan { get; set; }
    public float? TongDiem { get; set; }
    public string? NhanXet { get; set; }
    public DateTime NgayNop { get; set; }
}
