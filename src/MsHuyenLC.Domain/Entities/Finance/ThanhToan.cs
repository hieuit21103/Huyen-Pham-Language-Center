namespace MsHuyenLC.Domain.Entities.Finance;

public class ThanhToan
{
    public Guid Id { get; set; }
    
    // Thông tin cơ bản
    public string? MaThanhToan { get; set; }
    public decimal SoTien { get; set; }
    
    // Phương thức & Trạng thái
    public PhuongThucThanhToan PhuongThuc { get; set; } = PhuongThucThanhToan.tructuyen;
    public TrangThaiThanhToan TrangThai { get; set; } = TrangThaiThanhToan.chuathanhtoan;
    
    // Thời gian
    public DateOnly NgayLap { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly NgayHetHan { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
    public DateOnly? NgayThanhToan { get; set; }
    
    // Thông tin thanh toán online
    public string? ThongTinNganHang { get; set; } // Thông tin ngân hàng, STK
    public string? MaGiaoDichNganHang { get; set; } // Mã giao dịch từ ngân hàng
    public string? CongThanhToan { get; set; } // VNPay, MoMo, ZaloPay, etc.
    
    // Relationships - Liên kết với DangKy
    public Guid DangKyId { get; set; }
    public DangKy DangKy { get; set; } = null!;
    
}
