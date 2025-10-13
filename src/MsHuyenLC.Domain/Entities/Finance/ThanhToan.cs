namespace MsHuyenLC.Domain.Entities.Finance;

public class ThanhToan
{
    public Guid Id { get; set; }
    public decimal SoTien { get; set; }
    public PhuongThucThanhToan PhuongThuc { get; set; } = PhuongThucThanhToan.tructuyen;
    public TrangThaiThanhToan TrangThai { get; set; } = TrangThaiThanhToan.chuathanhtoan;
    public DateTime NgayLap { get; set; } = DateTime.UtcNow;
    public DateTime NgayHetHan { get; set; } = DateTime.UtcNow.AddDays(14);
    public DateTime? NgayThanhToan { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public KhoaHoc KhoaHoc { get; set; } = null!;
}
