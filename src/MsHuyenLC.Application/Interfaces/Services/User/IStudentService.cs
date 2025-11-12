using MsHuyenLC.Application.DTOs.Users.HocVien;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces.Services.User;

public interface IStudentService
{
    Task<HocVien?> GetByIdAsync(string id);
    Task<IEnumerable<HocVien>> GetAllAsync();
    Task<HocVien> CreateAsync(HocVienRequest request);
    Task<HocVien?> UpdateAsync(string id, HocVienUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<HocVien?> GetByAccountIdAsync(string accountId);
    Task<int> CountAsync();
}
