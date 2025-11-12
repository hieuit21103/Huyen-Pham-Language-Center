using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Services.Users;

/// <summary>
/// Service quản lý profile người dùng
/// </summary>
public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<TaiKhoanUpdateRequest> _updateValidator;

    public ProfileService(
        IUnitOfWork unitOfWork,
        IValidator<TaiKhoanUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Lấy thông tin profile theo ID người dùng
    /// </summary>
    public async Task<TaiKhoan?> GetProfileAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        return await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
    }

    /// <summary>
    /// Cập nhật thông tin profile
    /// </summary>
    public async Task<TaiKhoan?> UpdateProfileAsync(string userId, TaiKhoanUpdateRequest request)
    {
        // Validate request
        await _updateValidator.ValidateAndThrowAsync(request);

        // Lấy thông tin người dùng hiện tại
        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        if (user == null)
            return null;

        // Cập nhật thông tin
        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrEmpty(request.Sdt))
            user.Sdt = request.Sdt;

        if (!string.IsNullOrEmpty(request.Avatar))
            user.Avatar = request.Avatar;

        // Lưu thay đổi
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Thay đổi mật khẩu
    /// </summary>
    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            return false;

        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        if (user == null)
            return false;

        // Verify current password (should use password hashing in production)
        if (user.MatKhau != currentPassword)
            return false;

        // Update password
        user.MatKhau = newPassword;
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Kiểm tra tồn tại tài khoản theo ID
    /// </summary>
    public async Task<bool> ExistsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return false;

        if (!Guid.TryParse(userId, out var guid))
            return false;

        return await _unitOfWork.TaiKhoans.ExistsAsync(u => u.Id == guid);
    }
}
