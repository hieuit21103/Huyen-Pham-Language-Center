using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

/// <summary>
/// Service quản lý đề thi (CRUD + Auto Generation)
/// </summary>
public interface IExamService
{
    // CRUD Operations
    Task<DeThi?> GetByIdAsync(string id);
    Task<IEnumerable<DeThi>> GetAllAsync();
    Task<DeThi> CreateAsync(DeThiRequest request);
    Task<DeThi?> UpdateAsync(string id, DeThiUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    
    // Query Operations
    Task<DeThi?> GetWithQuestionsAsync(string id);
    Task<IEnumerable<DeThi>> GetByCreatorAsync(Guid creatorId);
    Task<IEnumerable<DeThi>> GetByExamSessionAsync(Guid kyThiId);
    
    // Auto Generation
    Task<DeThi> GenerateExamAsync(GenerateExamRequest request);
    Task<DeThi> GeneratePracticeTestAsync(
        CapDo capDo,
        DoKho doKho,
        KyNang kyNang,
        int soCauHoi,
        int thoiLuongPhut,
        Guid nguoiTaoId,
        CheDoCauHoi cheDoCauHoi = CheDoCauHoi.Don);
}
