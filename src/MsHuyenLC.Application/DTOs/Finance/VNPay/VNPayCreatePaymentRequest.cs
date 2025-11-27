namespace MsHuyenLC.Application.DTOs.Finance.VNPay;

public class VNPayCreatePaymentRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? BankCode { get; set; }
}
