using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;
using MsHuyenLC.Infrastructure.Services;
using MsHuyenLC.Application.Interfaces.Finance;
using MsHuyenLC.Application.DTOs.Finance.VNPay;
using MsHuyenLC.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace MsHuyenLC.API.Controller.Finance;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ThanhToanController : BaseController<ThanhToan>
{

    private readonly IVnpayService _vnpayService;
    private readonly ILogger<ThanhToanController> _logger;
    private readonly IGenericService<DangKy> _dangKyService;

    public ThanhToanController(
        IGenericService<ThanhToan> service,
        ISystemLoggerService loggerService,
        IVnpayService vnpayService,
        ILogger<ThanhToanController> logger,
        IGenericService<DangKy> dangKyService
        ) : base(service, loggerService)
    {
        _vnpayService = vnpayService;
        _logger = logger;
        _dangKyService = dangKyService;
    }

    [HttpGet("create/{id}")]
    public async Task<IActionResult> CreateUrl(string id, [FromQuery] string returnUrl)
    {
        var existingPayment = await _service.GetByIdAsync(id);
        if (existingPayment == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông tin thanh toán."
            });
        }

        var ipAddress = GetClientIpAddress();

        var request = new VNPayCreatePaymentRequest
        {
            OrderId = existingPayment.Id,
            Amount = existingPayment.SoTien,
            OrderInfo = $"Thanh toan cho ma dang ky {existingPayment.DangKyId}",
            ReturnUrl = returnUrl,
            IpAddress = ipAddress
        };

        var paymentUrl = await _vnpayService.CreatePaymentUrlAsync(request);
        if (paymentUrl == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Không thể tạo URL thanh toán."
            });
        }
        return Ok(new
        {
            success = true,
            message = "Tạo URL thanh toán thành công.",
            data = paymentUrl
        });
    }

    /// <summary>
    /// VNPay Return URL - Xử lý khi khách hàng quay lại từ VNPay
    /// </summary>
    [HttpPost("return")]
    [AllowAnonymous]
    public async Task<IActionResult> VNPayReturn([FromBody] VNPayCallbackRequest callbackRequest)
    {
        try
        {
            _logger.LogInformation("VNPay Return callback received for order: {OrderId}", callbackRequest.vnp_TxnRef);

            var result = await _vnpayService.ProcessCallbackAsync(callbackRequest);

            if (!result.Success)
            {
                _logger.LogWarning("VNPay payment failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage,
                    data = result
                });
            }

            // Cập nhật trạng thái thanh toán
            if (Guid.TryParse(result.OrderId, out var orderId))
            {
                var payment = await _service.GetByIdAsync(orderId.ToString());
                if (payment != null)
                {
                    payment.TrangThai = TrangThaiThanhToan.dathanhtoan;
                    payment.MaGiaoDichNganHang = result.TransactionId;
                    payment.ThongTinNganHang = result.BankCode;
                    payment.NgayThanhToan = DateOnly.FromDateTime(DateTime.Now);
                    payment.CongThanhToan = "VNPay";

                    await _service.UpdateAsync(payment);
                    
                    // Cập nhật trạng thái đăng ký sang "đã thanh toán"
                    if (payment.DangKyId != Guid.Empty)
                    {
                        var dangKy = await _dangKyService.GetByIdAsync(payment.DangKyId.ToString());
                        if (dangKy != null)
                        {
                            dangKy.TrangThai = TrangThaiDangKy.dathanhtoan; // 2 - đã thanh toán
                            await _dangKyService.UpdateAsync(dangKy);
                            _logger.LogInformation("Registration {DangKyId} status updated to dathanhtoan", payment.DangKyId);
                        }
                    }
                    
                    _logger.LogInformation("Payment {OrderId} updated successfully", orderId);
                }
            }

            return Ok(new
            {
                success = true,
                message = "Thanh toán thành công",
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay return callback");
            return StatusCode(500, new
            {
                success = false,
                message = "Lỗi xử lý callback từ VNPay"
            });
        }
    }

    /// <summary>
    /// VNPay IPN - Instant Payment Notification từ VNPay server
    /// </summary>
    [HttpGet("ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> VNPayIPN([FromQuery] VNPayCallbackRequest callbackRequest)
    {
        try
        {
            _logger.LogInformation("VNPay IPN received for order: {OrderId}", callbackRequest.vnp_TxnRef);

            var result = await _vnpayService.ProcessCallbackAsync(callbackRequest);

            if (!result.Success)
            {
                _logger.LogWarning("VNPay IPN validation failed: {ErrorMessage}", result.ErrorMessage);
                return Ok(new { message = result.ErrorMessage });
            }

            if (Guid.TryParse(result.OrderId, out var orderId))
            {
                var payment = await _service.GetByIdAsync(orderId.ToString());
                
                if (payment == null)
                {
                    _logger.LogWarning("Payment not found: {OrderId}", orderId);
                    return Ok(new { message = "Không tìm thấy đơn hàng" });
                }

                if (payment.TrangThai == TrangThaiThanhToan.dathanhtoan)
                {
                    _logger.LogInformation("Payment {OrderId} already processed", orderId);
                    return Ok(new { message = "Đơn hàng đã được thanh toán" });
                }

                if (payment.SoTien != result.Amount)
                {
                    _logger.LogWarning("Amount mismatch for order {OrderId}. Expected: {Expected}, Received: {Received}", 
                        orderId, payment.SoTien, result.Amount);
                    return Ok(new { message = "Số tiền không hợp lệ" });
                }

                payment.TrangThai = TrangThaiThanhToan.dathanhtoan;
                payment.MaGiaoDichNganHang = result.TransactionId;
                payment.ThongTinNganHang = result.BankCode;
                payment.NgayThanhToan = DateOnly.FromDateTime(DateTime.Now);
                payment.CongThanhToan = "VNPay";

                await _service.UpdateAsync(payment);

                // Cập nhật trạng thái đăng ký sang "đã thanh toán"
                if (payment.DangKyId != Guid.Empty)
                {
                    var dangKy = await _dangKyService.GetByIdAsync(payment.DangKyId.ToString());
                    if (dangKy != null)
                    {
                        dangKy.TrangThai = TrangThaiDangKy.dathanhtoan; // 2 - đã thanh toán
                        await _dangKyService.UpdateAsync(dangKy);
                        _logger.LogInformation("IPN: Registration {DangKyId} status updated to dathanhtoan", payment.DangKyId);
                    }
                }

                _logger.LogInformation("Payment {OrderId} confirmed via IPN", orderId);

                return Ok(new { message = "Xác nhận thành công" });
            }

            return Ok(new { message = "Lỗi không xác định" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay IPN");
            return Ok(new { message = "Lỗi hệ thống" });
        }
    }
}