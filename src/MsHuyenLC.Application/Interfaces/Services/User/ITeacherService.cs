using MsHuyenLC.Application.DTOs.Users.GiaoVien;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces.Services.User;

public interface ITeacherService
{
    Task<GiaoVien?> GetByIdAsync(string id);
    Task<IEnumerable<GiaoVien>> GetAllAsync();
    Task<GiaoVien> CreateAsync(GiaoVienRequest request);
    Task<GiaoVien?> UpdateAsync(string id, GiaoVienUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<GiaoVien?> GetByAccountIdAsync(string accountId);
    Task<int> CountAsync();
}
