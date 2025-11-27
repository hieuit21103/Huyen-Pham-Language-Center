using MsHuyenLC.Application.DTOs.Finance.ThanhToan;
using MsHuyenLC.Application.DTOs.Finance.VNPay;
using MsHuyenLC.Domain.Entities.Finance;

namespace MsHuyenLC.Application.Interfaces.Services.Finance;

public interface IPaymentService
{
    Task<ThanhToan?> GetByIdAsync(string id);
    Task<IEnumerable<ThanhToan>> GetAllAsync();
    Task<ThanhToan> CreateAsync(ThanhToanRequest request);
    Task<ThanhToan?> UpdateAsync(string id, ThanhToanUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<ThanhToan?> GetByRegistrationIdAsync(string registrationId);
    Task<IEnumerable<ThanhToan>> GetByStudentIdAsync(string studentId);
    Task<int> CountAsync();
    Task<ThanhToan?> ProcessVNPayCallbackAsync(VNPayCallbackResponse callbackResult);
}
