using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using MsHuyenLC.Application.DTOs.Finance.VNPay;
using MsHuyenLC.Application.Interfaces.Finance;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.Extensions.Logging;

namespace MsHuyenLC.Infrastructure.Services;

public class VNPayService : IVnpayService
{
    private readonly string _tmnCode;
    private readonly string _hashSecret;
    private readonly string _baseUrl;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VNPayService> _logger;

    public VNPayService(IConfiguration configuration, ILogger<VNPayService> logger)
    {
        _configuration = configuration;
        _tmnCode = _configuration["VNPay:TmnCode"] ?? Environment.GetEnvironmentVariable("Vnpay__TmnCode") ?? "";
        _logger = logger;
        _hashSecret = _configuration["VNPay:HashSecret"] ?? Environment.GetEnvironmentVariable("Vnpay__HashSecret") ?? "";
        _baseUrl = _configuration["VNPay:BaseUrl"] ?? Environment.GetEnvironmentVariable("Vnpay__BaseUrl") ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    }

    public Task<VNPayPaymentResponse> CreatePaymentUrlAsync(VNPayCreatePaymentRequest request)
    {
        try
        {
            var vnpayData = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _tmnCode },
                { "vnp_Amount", ((long)(request.Amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", request.IpAddress ?? "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", request.OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", request.ReturnUrl },
                { "vnp_TxnRef", request.OrderId.ToString() }
            };

            var queryBuilder = new StringBuilder();
            var hashDataBuilder = new StringBuilder();

            foreach (var kv in vnpayData.Where(x => !string.IsNullOrEmpty(x.Value)))
            {
                if (hashDataBuilder.Length > 0)
                {
                    hashDataBuilder.Append('&');
                    queryBuilder.Append('&');
                }

                hashDataBuilder.Append($"{kv.Key}={WebUtility.UrlEncode(kv.Value)}");
                queryBuilder.Append($"{kv.Key}={WebUtility.UrlEncode(kv.Value)}");
            }

            var rawHash = hashDataBuilder.ToString();
            var queryString = queryBuilder.ToString();
            var vnpSecureHash = HmacSHA512(_hashSecret, rawHash);

            var paymentUrl = $"{_baseUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";

            return Task.FromResult(new VNPayPaymentResponse
            {
                Success = true,
                PaymentUrl = paymentUrl
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new VNPayPaymentResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            });
        }
    }

    public Task<VNPayCallbackResponse> ProcessCallbackAsync(VNPayCallbackRequest request)
    {
        try
        {
            if (!ValidateSignature(request))
            {
                return Task.FromResult(new VNPayCallbackResponse
                {
                    Success = false,
                    ErrorMessage = "Chữ ký không hợp lệ"
                });
            }

            var isSuccess = request.vnp_ResponseCode == "00" && request.vnp_TransactionStatus == "00";

            var response = new VNPayCallbackResponse
            {
                Success = isSuccess,
                OrderId = request.vnp_TxnRef,
                Amount = decimal.Parse(request.vnp_Amount) / 100, // Chia cho 100 để ra số tiền thực
                TransactionId = request.vnp_TransactionNo,
                BankCode = request.vnp_BankCode,
                BankTranNo = request.vnp_BankTranNo,
                CardType = request.vnp_CardType,
                PayDate = request.vnp_PayDate,
                ResponseCode = request.vnp_ResponseCode,
                ErrorMessage = isSuccess ? null : GetResponseMessage(request.vnp_ResponseCode)
            };

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            return Task.FromResult(new VNPayCallbackResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            });
        }
    }

    public bool ValidateSignature(VNPayCallbackRequest request)
    {
        try
        {
            var vnPay = new SortedDictionary<string, string>();

            foreach (var kv in request.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(request)?.ToString() ?? ""))
            {
                if (!string.IsNullOrEmpty(kv.Value) && kv.Key.StartsWith("vnp_"))
                {
                    vnPay.Add(kv.Key, kv.Value);
                }
            }

            vnPay.Remove("vnp_SecureHashType");
            vnPay.Remove("vnp_SecureHash");

            var hashData = new StringBuilder();
            foreach (var kv in vnPay)
            {
                if (hashData.Length > 0)
                {
                    hashData.Append('&');
                }
                hashData.Append($"{kv.Key}={WebUtility.UrlEncode(kv.Value)}");
            }
            var inputHash = request.vnp_SecureHash;
            var checkSum = HmacSHA512(_hashSecret, hashData.ToString());
            return checkSum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private string HmacSHA512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private string GetResponseMessage(string responseCode)
    {
        return responseCode switch
        {
            "00" => "Giao dịch thành công",
            "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
            "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
            "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
            "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
            "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
            "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).",
            "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
            "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
            "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
            "75" => "Ngân hàng thanh toán đang bảo trì.",
            "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.",
            _ => "Giao dịch không thành công"
        };
    }
}