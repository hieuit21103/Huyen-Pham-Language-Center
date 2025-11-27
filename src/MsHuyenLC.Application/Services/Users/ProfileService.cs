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

    public async Task<TaiKhoan?> GetProfileAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        return await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
    }

    public async Task<TaiKhoan?> UpdateProfileAsync(string userId, TaiKhoanUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        if (user == null)
            return null;

        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrEmpty(request.Sdt))
            user.Sdt = request.Sdt;

        if (!string.IsNullOrEmpty(request.Avatar))
            user.Avatar = request.Avatar;

        await _unitOfWork.SaveChangesAsync();

        return user;
    }
    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            return false;

        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        if (user == null)
            return false;

        if (user.MatKhau != currentPassword)
            return false;

        user.MatKhau = newPassword;
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return false;

        if (!Guid.TryParse(userId, out var guid))
            return false;

        return await _unitOfWork.TaiKhoans.ExistsAsync(u => u.Id == guid);
    }
}
