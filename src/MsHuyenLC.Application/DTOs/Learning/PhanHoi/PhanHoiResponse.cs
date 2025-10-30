namespace MsHuyenLC.Application.DTOs.Learning.PhanHoi;

public class PhanHoiResponse
{
    public Guid Id { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public string LoaiPhanHoi { get; set; } = null!;
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public DateOnly NgayTao { get; set; }
}
