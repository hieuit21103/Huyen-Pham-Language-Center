using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface ITestService
{
    Task<DeThi?> GetByIdAsync(string id);
    Task<IEnumerable<DeThi>> GetAllAsync();
    Task<DeThi> CreateAsync(DeThiRequest request);
    Task<DeThi?> UpdateAsync(string id, DeThiUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<DeThi?> GetTestWithQuestionsAsync(string id);
    Task<DeThi> AddQuestionToTestAsync(string testId, string questionId);
    Task<bool> RemoveQuestionFromTestAsync(string testId, string questionId);
    Task<IEnumerable<DeThi>> GetTestsByCreatorAsync(string creatorId);
    Task<IEnumerable<DeThi>> GetTestsByExamAsync(string examId);
    Task<DeThi> GenerateTestAsync(GenerateTestRequest request);
    Task<DeThi> GenerateTestWithDifficultyDistributionAsync(GenerateTestWithDifficultyRequest request);
    Task<DeThi> CreateMixedTestAsync(CreateMixedTestRequest request);
    Task<object?> GetTestWithQuestionsGroupedAsync(string id);
}
