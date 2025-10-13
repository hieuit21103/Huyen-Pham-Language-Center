namespace MsHuyenLC.Domain.Entities.System;

public class NhatKyHeThong
{
    public Guid Id { get; set; }
    public DateTime ThoiGian { get; set; } = DateTime.UtcNow;
    public string HanhDong { get; set; } = null!;
    public string? ChiTiet { get; set; }
    public string DuLieuCu { get; set; } = null!;
    public string DuLieuMoi { get; set; } = null!;
    public string IP { get; set; } = null!;
    public TaiKhoan TaiKhoan { get; set; } = null!;

}