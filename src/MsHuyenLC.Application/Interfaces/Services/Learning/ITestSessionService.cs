using MsHuyenLC.Application.DTOs.Learning.PhienLamBai;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.Interfaces.Services.Learning;

public interface ITestSessionService
{
    Task<PhienLamBai?> GetByIdAsync(string id);
    Task<IEnumerable<PhienLamBai>> GetAllAsync();
    Task<IEnumerable<PhienLamBai>> GetByHocVienIdAsync(string hocVienId);
    Task<IEnumerable<PhienLamBai>> GetByDeThiIdAsync(string deThiId);
    Task<PhienLamBai?> SubmitAsync(SubmitRequest request, Guid taiKhoanId);
    Task<PhienLamBai?> GradeAsync(string id, GradingRequest request);
    Task<PhienLamBaiResponse?> GetDetailsAsync(string id);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<bool> HasSubmittedAsync(string hocVienId, string deThiId);
}
