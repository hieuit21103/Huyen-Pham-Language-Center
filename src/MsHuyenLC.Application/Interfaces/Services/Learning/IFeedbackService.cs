using MsHuyenLC.Application.DTOs.Learning.PhanHoi;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IFeedbackService
{
    Task<PhanHoi?> GetByIdAsync(string id);
    Task<IEnumerable<PhanHoi>> GetAllAsync();
    Task<PhanHoi> CreateAsync(PhanHoiRequest request);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<PhanHoi>> GetByStudentIdAsync(string studentId);
    Task<int> CountAsync();
}
