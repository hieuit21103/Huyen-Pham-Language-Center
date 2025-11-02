namespace MsHuyenLC.Application.DTOs.Finance.VNPay;

public class VNPayPaymentResponse
{
    public bool Success { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
