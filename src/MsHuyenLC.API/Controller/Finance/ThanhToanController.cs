using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.Finance;
using MsHuyenLC.Application.DTOs.Finance.VNPay;
using System.Security.Claims; 

namespace MsHuyenLC.API.Controller.Finance;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ThanhToanController : BaseController
{

    private readonly IVnpayService _vnpayService;
    private readonly IPaymentService _service;

    public ThanhToanController(
        ISystemLoggerService loggerService,
        IPaymentService service,
        IVnpayService vnpayService
        ) : base(loggerService)
    {
        _service = service;
        _vnpayService = vnpayService;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _service.GetAllAsync();

        var totalItems = await _service.CountAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách thanh toán thành công",
            count = totalItems,
            data = payments
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetById(string id)
    {
        var payment = await _service.GetByIdAsync(id);
        if (payment == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông tin thanh toán"
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin thanh toán thành công",
            data = payment
        });
    }

    [HttpGet("dang-ky/{dangKyId}")]
    [Authorize]
    public async Task<IActionResult> GetByRegistrationId(string dangKyId)
    {
        var payment = await _service.GetByRegistrationIdAsync(dangKyId);
        if (payment == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông tin thanh toán"
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin thanh toán thành công",
            data = payment
        });
    }

    [HttpGet("student/{studentId}")]
    [Authorize]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var payments = await _service.GetByStudentIdAsync(studentId);
        if (payments == null || !payments.Any())
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông tin thanh toán"
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin thanh toán thành công",
            data = payments
        });
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
            var callbackResult = await _vnpayService.ProcessCallbackAsync(callbackRequest);

            if (!callbackResult.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = callbackResult.ErrorMessage,
                    data = callbackResult
                });
            }

            var payment = await _service.ProcessVNPayCallbackAsync(callbackResult);
            
            if (payment == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không thể xử lý thanh toán"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Thanh toán thành công",
                data = callbackResult
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Lỗi xử lý callback từ VNPay",
                error = ex.Message
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
            // Xác thực callback từ VNPay
            var callbackResult = await _vnpayService.ProcessCallbackAsync(callbackRequest);

            if (!callbackResult.Success)
            {
                return Ok(new 
                { 
                    RspCode = "97",
                    Message = callbackResult.ErrorMessage 
                });
            }

            if (Guid.TryParse(callbackResult.OrderId, out var orderId))
            {
                var payment = await _service.GetByIdAsync(orderId.ToString());

                if (payment == null)
                {
                    return Ok(new 
                    { 
                        RspCode = "01",
                        Message = "Không tìm thấy đơn hàng" 
                    });
                }

                if (payment.TrangThai == TrangThaiThanhToan.dathanhtoan)
                {
                    return Ok(new 
                    { 
                        RspCode = "02",
                        Message = "Đơn hàng đã được thanh toán" 
                    });
                }

                if (payment.SoTien != callbackResult.Amount)
                {
                    return Ok(new 
                    { 
                        RspCode = "04",
                        Message = "Số tiền không hợp lệ" 
                    });
                }

                // Xử lý logic nghiệp vụ trong service layer
                var processedPayment = await _service.ProcessVNPayCallbackAsync(callbackResult);
                
                if (processedPayment == null)
                {
                    return Ok(new 
                    { 
                        RspCode = "99",
                        Message = "Lỗi xử lý thanh toán" 
                    });
                }

                return Ok(new 
                { 
                    RspCode = "00",
                    Message = "Xác nhận thành công" 
                });
            }

            return Ok(new 
            { 
                RspCode = "99",
                Message = "Lỗi không xác định" 
            });
        }
        catch (Exception)
        {
            return Ok(new 
            { 
                RspCode = "99",
                Message = "Lỗi hệ thống" 
            });
        }
    }
}

