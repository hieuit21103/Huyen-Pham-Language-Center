using MsHuyenLC.Application.DTOs.Learning.DangKyKhoaHoc;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IRegistrationService
{
    Task<DangKyKhoaHoc?> GetByIdAsync(string id, string userId = "");
    Task<IEnumerable<DangKyKhoaHoc>> GetAllAsync();
    Task<DangKyKhoaHoc> CreateAsync(DangKyKhoaHocRequest request);
    Task<DangKyKhoaHoc> CreateFullAsync(DangKyKhoaHocCreateRequest request);
    Task<DangKyKhoaHoc?> UpdateAsync(string id, DangKyKhoaHocUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<DangKyKhoaHoc>> GetByStudentIdAsync(string studentId);
    Task<IEnumerable<DangKyKhoaHoc>> GetByCourseIdAsync(string courseId);
    Task<int> CountAsync();
}
