using MsHuyenLC.Application.DTOs.Learning.DangKyTuVan;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IGuestRegistrationService
{
    Task<DangKyTuVan?> GetByIdAsync(string id);
    Task<IEnumerable<DangKyTuVan>> GetAllAsync();
    Task<DangKyTuVan> CreateAsync(DangKyTuVanRequest request);
    Task<DangKyTuVan?> CreateFullAsync(DangKyTuVanCreateRequest request);
    Task<DangKyTuVan?> UpdateAsync(string id, DangKyTuVanUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<DangKyTuVan>> GetByCourseIdAsync(string courseId);
    Task<int> CountAsync();
}
