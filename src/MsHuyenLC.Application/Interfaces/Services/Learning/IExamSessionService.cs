using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IExamSessionService
{
    Task<KyThi?> GetByIdAsync(string id);
    Task<IEnumerable<KyThi>> GetAllAsync();
    Task<KyThi> CreateAsync(KyThiRequest request);
    Task<KyThi?> UpdateAsync(string id, KyThiUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<KyThi>> GetByClassIdAsync(string classId);
    Task<IEnumerable<KyThi>> GetByStudentIdAsync(Guid studentId);
    Task<Guid> JoinExamAsync(JoinExamRequest request);
    Task<int> CountAsync();
}
