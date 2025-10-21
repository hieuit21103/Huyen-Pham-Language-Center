namespace MsHuyenLC.Domain.Entities.Finance;

public class ThanhToan
{
    public Guid Id { get; set; }
    public decimal SoTien { get; set; }
    public PhuongThucThanhToan PhuongThuc { get; set; } = PhuongThucThanhToan.tructuyen;
    public TrangThaiThanhToan TrangThai { get; set; } = TrangThaiThanhToan.chuathanhtoan;
    public DateOnly NgayLap { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly NgayHetHan { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
    public DateOnly? NgayThanhToan { get; set; }
    public Guid HocVienId { get; set; }
    public Guid KhoaHocId { get; set; }
    public HocVien HocVien { get; set; } = null!;
    public KhoaHoc KhoaHoc { get; set; } = null!;
}
