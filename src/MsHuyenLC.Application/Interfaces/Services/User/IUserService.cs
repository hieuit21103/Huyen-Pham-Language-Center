using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces.Services.User;

public interface IUserService
{
    Task<TaiKhoan?> GetUserByIdAsync(string userId);
    Task<IEnumerable<TaiKhoan?>> GetAllUsersAsync();
    Task<string> GetCurrentUserIpAddress();
    Task<TaiKhoan?> CreateUserAsync(TaiKhoanRequest request);
    Task<TaiKhoan?> UpdateUserAsync(string userId, TaiKhoanUpdateRequest updateUserDto);
    Task<bool> DeleteUserAsync(string userId);
    Task<int> CountUsersAsync();
    Task<bool> ExistByIdAsync(string userId);
    Task<bool> ExistByUsernameAsync(string username);
    Task<bool> ExistByEmailAsync(string email);

}
