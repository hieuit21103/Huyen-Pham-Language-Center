using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Finance;

public class ThanhToan
{
    public Guid Id { get; set; }
    
    // Thông tin cơ bản
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
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
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string? ThongTinNganHang { get; set; } // Thông tin ngân hàng, STK
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string? MaGiaoDichNganHang { get; set; } // Mã giao dịch từ ngân hàng
    
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? CongThanhToan { get; set; } // VNPay, MoMo, ZaloPay, etc.
    
    // Relationships - Liên kết với DangKy
    public Guid DangKyId { get; set; }
    public DangKyKhoaHoc DangKy { get; set; } = null!;
    
}
