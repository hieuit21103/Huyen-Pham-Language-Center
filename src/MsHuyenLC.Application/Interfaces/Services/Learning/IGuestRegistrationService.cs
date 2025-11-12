using MsHuyenLC.Application.DTOs.Learning.DangKyKhach;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IGuestRegistrationService
{
    Task<DangKyKhach?> GetByIdAsync(string id);
    Task<IEnumerable<DangKyKhach>> GetAllAsync();
    Task<DangKyKhach> CreateAsync(DangKyKhachRequest request);
    Task<DangKyKhach?> CreateFullAsync(DangKyKhachCreateRequest request);
    Task<DangKyKhach?> UpdateAsync(string id, DangKyKhachUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<DangKyKhach>> GetByCourseIdAsync(string courseId);
    Task<int> CountAsync();
}
