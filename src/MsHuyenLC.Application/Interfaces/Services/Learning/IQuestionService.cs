using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IQuestionService
{
    Task<CauHoi?> GetByIdAsync(string id);
    Task<IEnumerable<CauHoi>> GetAllAsync();
    Task<CauHoi> CreateAsync(CauHoiRequest request);
    Task<CauHoi?> UpdateAsync(string id, CauHoiUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<IEnumerable<CauHoi>> GetBySkillAsync(KyNang skill);
    Task<IEnumerable<CauHoi>> GetByDifficultyAsync(DoKho difficulty);
    Task<IEnumerable<CauHoi>> GetByLevelAsync(CapDo level);
    Task<IEnumerable<CauHoi>> GetByTypeAsync(LoaiCauHoi type);
    Task<CauHoi> AddAnswerToQuestionAsync(string questionId, DapAnRequest answerRequest);
    Task<CauHoi?> GetQuestionWithAnswersAsync(string id);
}
