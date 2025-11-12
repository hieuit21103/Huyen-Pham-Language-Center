using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface IGroupQuestionService
{
    Task<IEnumerable<NhomCauHoiResponse>> GetAllAsync();
    Task<NhomCauHoiResponse?> GetByIdAsync(string id);
    Task<NhomCauHoiResponse> CreateAsync(NhomCauHoiRequest request);
    Task<NhomCauHoiResponse> UpdateAsync(string id, NhomCauHoiUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<NhomCauHoiResponse> AddQuestionToGroupAsync(string groupId, string questionId, int? thuTu = null);
    Task<List<CauHoiResponse>> GetQuestionsByGroupIdAsync(string groupId);
}
