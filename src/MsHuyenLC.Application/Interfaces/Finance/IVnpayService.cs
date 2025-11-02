using MsHuyenLC.Application.DTOs.Finance.VNPay;

namespace MsHuyenLC.Application.Interfaces.Finance;

public interface IVnpayService
{
    /// <summary>
    /// Tạo URL thanh toán VNPay
    /// </summary>
    /// <param name="request">Thông tin yêu cầu thanh toán</param>
    /// <returns>URL thanh toán VNPay</returns>
    Task<VNPayPaymentResponse> CreatePaymentUrlAsync(VNPayCreatePaymentRequest request);

    /// <summary>
    /// Xử lý callback từ VNPay sau khi thanh toán
    /// </summary>
    /// <param name="request">Thông tin callback từ VNPay</param>
    /// <returns>Kết quả xử lý callback</returns>
    Task<VNPayCallbackResponse> ProcessCallbackAsync(VNPayCallbackRequest request);

    /// <summary>
    /// Xác thực chữ ký từ VNPay
    /// </summary>
    /// <param name="request">Thông tin callback từ VNPay</param>
    /// <returns>True nếu chữ ký hợp lệ</returns>
    bool ValidateSignature(VNPayCallbackRequest request);
}
