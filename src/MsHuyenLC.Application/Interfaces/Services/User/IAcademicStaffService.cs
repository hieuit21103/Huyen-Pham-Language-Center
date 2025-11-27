using MsHuyenLC.Application.DTOs.Users.GiaoVu;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces.Services.User;

public interface IAcademicStaffService
{
    Task<GiaoVu?> GetByIdAsync(string id);
    Task<IEnumerable<GiaoVu>> GetAllAsync();
    Task<GiaoVu?> CreateAsync(GiaoVuRequest request);
    Task<GiaoVu?> UpdateAsync(string id, GiaoVuUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<GiaoVu?> GetByAccountIdAsync(string accountId);
    Task<int> CountAsync();
}
