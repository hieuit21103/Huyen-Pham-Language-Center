using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces.Services.User;
public interface IProfileService
{
    Task<TaiKhoan?> GetProfileAsync(string userId);
    Task<TaiKhoan?> UpdateProfileAsync(string userId, TaiKhoanUpdateRequest request);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<bool> ExistsAsync(string userId);
}
