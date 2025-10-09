namespace MsHuyenLC.Domain.Entities.Finance;

public class ThanhToan
{
    public Guid Id { get; set; }
    public decimal SoTien { get; set; }
    public PhuongThucThanhToan PhuongThuc { get; set; } = PhuongThucThanhToan.tructuyen;
    public TrangThaiThanhToan TrangThai { get; set; } = TrangThaiThanhToan.choduyet;
    public DateTime NgayThanhToan { get; set; } = DateTime.Now;

    public HocVien HocVien { get; set; } = null!;
    public KhoaHoc KhoaHoc { get; set; } = null!;
}
