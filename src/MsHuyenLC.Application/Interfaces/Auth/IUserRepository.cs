using MsHuyenLC.Domain.Entities.Users;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Interfaces.Auth;

public interface IUserRepository
{
    Task<TaiKhoan?> GetByUsernameAsync(string username);
    Task<TaiKhoan?> GetByEmailAsync(string email);
    Task<TaiKhoan?> GetByIdAsync(string userId);
    Task<TaiKhoan> CreateAsync(TaiKhoan user);
    Task UpdateAsync(TaiKhoan user);
    Task DeleteAsync(string userId);
    Task<bool> UserExistsAsync(string username, string email);
    Task AssignRoleAsync(string userId, VaiTro role);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task SaveChangesAsync();
}