using FluentValidation;
using MsHuyenLC.Application.DTOs.Finance.ThanhToan;
using MsHuyenLC.Application.DTOs.Finance.VNPay;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Finance;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Domain.Entities.Finance;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Services.Finance;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ThanhToanRequest> _createValidator;
    private readonly IValidator<ThanhToanUpdateRequest> _updateValidator;
    private readonly IRegistrationService _registrationService;

    public PaymentService(
        IUnitOfWork unitOfWork,
        IValidator<ThanhToanRequest> createValidator,
        IValidator<ThanhToanUpdateRequest> updateValidator,
        IRegistrationService registrationService)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _registrationService = registrationService;
    }

    public async Task<ThanhToan?> GetByIdAsync(string id)
    {
        return await _unitOfWork.ThanhToans.GetByIdAsync(id);
    }

    public async Task<IEnumerable<ThanhToan>> GetAllAsync()
    {
        return await _unitOfWork.ThanhToans.GetAllAsync();
    }

    public async Task<ThanhToan> CreateAsync(ThanhToanRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var thanhToan = new ThanhToan
        {
            DangKyId = request.DangKyId,
            SoTien = request.SoTien,
            NgayLap = DateOnly.FromDateTime(DateTime.UtcNow),
            NgayHetHan = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
            PhuongThuc = PhuongThucThanhToan.tructuyen,
            TrangThai = TrangThaiThanhToan.chuathanhtoan
        };

        var result = await _unitOfWork.ThanhToans.AddAsync(thanhToan);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<ThanhToan?> UpdateAsync(string id, ThanhToanUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var thanhToan = await _unitOfWork.ThanhToans.GetByIdAsync(id);
        if (thanhToan == null)
            throw new KeyNotFoundException($"Không tìm thấy thanh toán với ID: {id}");

        thanhToan.TrangThai = request.TrangThai;
        thanhToan.NgayThanhToan = request.NgayThanhToan;
        thanhToan.ThongTinNganHang = request.ThongTinNganHang;
        thanhToan.MaGiaoDichNganHang = request.MaGiaoDichNganHang;
        thanhToan.CongThanhToan = request.CongThanhToan;

        await _unitOfWork.SaveChangesAsync();
        return thanhToan;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var thanhToan = await _unitOfWork.ThanhToans.GetByIdAsync(id);
        if (thanhToan == null)
            return false;

        await _unitOfWork.ThanhToans.DeleteAsync(thanhToan);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ThanhToan>> GetByRegistrationIdAsync(string registrationId)
    {
        if (!Guid.TryParse(registrationId, out var id))
            return Enumerable.Empty<ThanhToan>();

        return await _unitOfWork.ThanhToans.GetAllAsync(t => t.DangKyId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.ThanhToans.CountAsync();
    }

    public async Task<ThanhToan?> ProcessVNPayCallbackAsync(VNPayCallbackResponse callbackResult)
    {
        if (!callbackResult.Success || string.IsNullOrEmpty(callbackResult.OrderId))
            return null;

        var payment = await _unitOfWork.ThanhToans.GetByIdAsync(callbackResult.OrderId);
        if (payment == null)
            return null;

        payment.MaThanhToan = callbackResult.TransactionId;
        payment.TrangThai = TrangThaiThanhToan.dathanhtoan;
        payment.MaGiaoDichNganHang = callbackResult.TransactionId;
        payment.ThongTinNganHang = callbackResult.BankCode;
        payment.NgayThanhToan = DateOnly.FromDateTime(DateTime.UtcNow);
        payment.CongThanhToan = "VNPay";

        await _unitOfWork.SaveChangesAsync();

        if (payment.DangKyId != Guid.Empty)
        {
            var dangKy = await _registrationService.GetByIdAsync(payment.DangKyId.ToString());
            if (dangKy != null)
            {
                dangKy.TrangThai = TrangThaiDangKy.dathanhtoan;
                await _registrationService.UpdateAsync(dangKy.Id.ToString(), new MsHuyenLC.Application.DTOs.Learning.DangKy.DangKyUpdateRequest
                {
                    TrangThai = TrangThaiDangKy.dathanhtoan
                });
            }
        }

        return payment;
    }
}
