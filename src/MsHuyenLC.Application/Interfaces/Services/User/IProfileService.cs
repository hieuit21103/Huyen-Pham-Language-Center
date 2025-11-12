using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Interfaces.Services.User;

/// <summary>
/// Interface cho service quản lý profile người dùng
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Lấy thông tin profile theo ID người dùng
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <returns>Thông tin tài khoản</returns>
    Task<TaiKhoan?> GetProfileAsync(string userId);

    /// <summary>
    /// Cập nhật thông tin profile
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="request">Dữ liệu cập nhật</param>
    /// <returns>Thông tin tài khoản đã cập nhật</returns>
    Task<TaiKhoan?> UpdateProfileAsync(string userId, TaiKhoanUpdateRequest request);

    /// <summary>
    /// Thay đổi mật khẩu
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="currentPassword">Mật khẩu hiện tại</param>
    /// <param name="newPassword">Mật khẩu mới</param>
    /// <returns>True nếu thành công</returns>
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    /// <summary>
    /// Kiểm tra tồn tại tài khoản theo ID
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <returns>True nếu tồn tại</returns>
    Task<bool> ExistsAsync(string userId);
}
