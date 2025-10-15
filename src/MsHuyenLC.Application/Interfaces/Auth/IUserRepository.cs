using System.Linq.Expressions;
using MsHuyenLC.Domain.Entities.Users;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Interfaces.Auth;

public interface IUserRepository
{
    Task<IEnumerable<TaiKhoan>> GetAllAsync(
        int PageNumber,
        int PageSize,
        Expression<Func<TaiKhoan, bool>>? Filter = null,
        Func<IQueryable<TaiKhoan>, IOrderedQueryable<TaiKhoan>>? OrderBy = null,
        params Expression<Func<TaiKhoan, object>>[] Includes
    );
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