namespace MsHuyenLC.Application.DTOs.Learning.ThongBao;

public class ThongBaoResponse
{
    public Guid Id { get; set; }
    public Guid NguoiGuiId { get; set; }
    public Guid? NguoiNhanId { get; set; }
    public string? TenNguoiGui { get; set; }
    public string? TenNguoiNhan { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public DateTime NgayTao { get; set; }
}
