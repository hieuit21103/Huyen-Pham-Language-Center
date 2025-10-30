namespace MsHuyenLC.Application.DTOs.Learning.ThongBao;

public class ThongBaoRequest
{
    public Guid NguoiGuiId { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
}
