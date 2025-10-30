namespace MsHuyenLC.Domain.Entities.Finance;

public class ThanhToan
{
    public Guid Id { get; set; }
    
    // Thông tin cơ bản
    public string MaThanhToan { get; set; } = null!; // TT-YYYYMMDD-XXXX
    public decimal SoTien { get; set; }
    public string? GhiChu { get; set; }
    
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
    
    // Relationships - FIX: Liên kết với DangKy thay vì HocVien + KhoaHoc
    public Guid DangKyId { get; set; }
    public DangKy DangKy { get; set; } = null!;
    
    // Audit trail
    public Guid? NguoiXacNhanId { get; set; } // GiaoVu xác nhận thanh toán
    public TaiKhoan? NguoiXacNhan { get; set; }
    public DateTime? NgayXacNhan { get; set; }
    public Guid? NguoiCapNhatId { get; set; }
    public TaiKhoan? NguoiCapNhat { get; set; }
    public DateTime? NgayCapNhat { get; set; }
}
