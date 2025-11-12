using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.TaiKhoan;
using MsHuyenLC.Domain.Entities.Users;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<TaiKhoanRequest> _createValidator;
    private readonly IValidator<TaiKhoanUpdateRequest> _updateValidator;

    public UserService(
        IUnitOfWork unitOfWork,
        IValidator<TaiKhoanRequest> createValidator,
        IValidator<TaiKhoanUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    public async Task<TaiKhoan?> GetUserByIdAsync(string userId)
    {
        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        return user ?? null;
    }

    public async Task<IEnumerable<TaiKhoan?>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.TaiKhoans.GetAllAsync();
        return users.Select(user => user);
    }

    public Task<string> GetCurrentUserIpAddress()
    {
        throw new NotImplementedException();
    }

    public async Task<TaiKhoan?> CreateUserAsync(TaiKhoanRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var newUser = new TaiKhoan
        {
            TenDangNhap = request.TenDangNhap,
            MatKhau = request.MatKhau,
            Email = request.Email,
            Sdt = request.Sdt,
            VaiTro = request.VaiTro,
            Avatar = request.Avatar,
            NgayTao = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        var result = await _unitOfWork.TaiKhoans.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<TaiKhoan?> UpdateUserAsync(string userId, TaiKhoanUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        if (user == null) return null;

        user.Email = request.Email;
        user.Sdt = request.Sdt;
        user.VaiTro = request.VaiTro;
        user.Avatar = request.Avatar;
        user.TrangThai = request.TrangThai;

        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _unitOfWork.TaiKhoans.GetByIdAsync(userId);
        if (user == null) return false;

        await _unitOfWork.TaiKhoans.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountUsersAsync()
    {
        return await _unitOfWork.TaiKhoans.CountAsync();
    }

    public async Task<bool> ExistByIdAsync(string userId)
    {
        return await _unitOfWork.TaiKhoans.ExistsAsync(u => u.Id == Guid.Parse(userId));
    }

    public async Task<bool> ExistByUsernameAsync(string username)
    {
        return await _unitOfWork.TaiKhoans.ExistsAsync(u => u.TenDangNhap == username);
    }

    public async Task<bool> ExistByEmailAsync(string email)
    {
        return await _unitOfWork.TaiKhoans.ExistsAsync(u => u.Email == email);
    }

    
}
