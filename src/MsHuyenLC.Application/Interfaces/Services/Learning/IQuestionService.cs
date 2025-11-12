using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IQuestionService
{
    Task<NganHangCauHoi?> GetByIdAsync(string id);
    Task<IEnumerable<NganHangCauHoi>> GetAllAsync();
    Task<NganHangCauHoi> CreateAsync(CauHoiRequest request);
    Task<NganHangCauHoi?> UpdateAsync(string id, CauHoiUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<IEnumerable<NganHangCauHoi>> GetBySkillAsync(KyNang skill);
    Task<IEnumerable<NganHangCauHoi>> GetByDifficultyAsync(DoKho difficulty);
    Task<IEnumerable<NganHangCauHoi>> GetByLevelAsync(CapDo level);
    Task<IEnumerable<NganHangCauHoi>> GetByTypeAsync(LoaiCauHoi type);
    Task<NganHangCauHoi> AddAnswerToQuestionAsync(string questionId, DapAnRequest answerRequest);
    Task<NganHangCauHoi?> GetQuestionWithAnswersAsync(string id);
}
