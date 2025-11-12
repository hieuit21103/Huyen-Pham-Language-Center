using MsHuyenLC.Application.DTOs.Learning.DangKy;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IRegistrationService
{
    Task<DangKy?> GetByIdAsync(string id);
    Task<IEnumerable<DangKy>> GetAllAsync();
    Task<DangKy> CreateAsync(DangKyRequest request);
    Task<DangKy> CreateFullAsync(DangKyCreateRequest request);
    Task<DangKy?> UpdateAsync(string id, DangKyUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<DangKy>> GetByStudentIdAsync(string studentId);
    Task<IEnumerable<DangKy>> GetByCourseIdAsync(string courseId);
    Task<int> CountAsync();
}
