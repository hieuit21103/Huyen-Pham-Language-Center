namespace MsHuyenLC.Application.DTOs.Finance.VNPay;

public class VNPayCallbackResponse
{
    public bool Success { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string BankTranNo { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string PayDate { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
