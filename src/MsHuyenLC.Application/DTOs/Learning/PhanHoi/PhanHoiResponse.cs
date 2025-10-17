namespace MsHuyenLC.Application.DTOs.Learning.PhanHoi;

public class PhanHoiResponse
{
    public Guid Id { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public Guid GiaoVienId { get; set; }
    public string? TenGiaoVien { get; set; }
    public string NoiDung { get; set; } = null!;
    public DateTime NgayGui { get; set; }
}
