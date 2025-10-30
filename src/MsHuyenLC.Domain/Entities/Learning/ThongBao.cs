namespace MsHuyenLC.Domain.Entities.Learning;

public class ThongBao
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public Guid NguoiGuiId { get; set; }
    public TaiKhoan NguoiGui { get; set; } = null!;
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
}