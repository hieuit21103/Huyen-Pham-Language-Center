using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IExamSessionService
{
    Task<KyThi?> GetByIdAsync(string id);
    Task<IEnumerable<KyThi>> GetAllAsync();
    Task<KyThi> CreateAsync(KyThiRequest request);
    Task<KyThi?> UpdateAsync(string id, KyThiUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<KyThi>> GetByClassIdAsync(string classId);
    Task<int> CountAsync();
}
