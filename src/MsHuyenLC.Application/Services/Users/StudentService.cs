using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.HocVien;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Services.Users;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<HocVienRequest> _createValidator;
    private readonly IValidator<HocVienUpdateRequest> _updateValidator;

    public StudentService(
        IUnitOfWork unitOfWork,
        IValidator<HocVienRequest> createValidator,
        IValidator<HocVienUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<HocVien?> GetByIdAsync(string id)
    {
        return await _unitOfWork.HocViens.GetByIdAsync(id);
    }

    public async Task<IEnumerable<HocVien>> GetAllAsync()
    {
        return await _unitOfWork.HocViens.GetAllAsync();
    }

    public async Task<HocVien> CreateAsync(HocVienRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var hocVien = new HocVien
        {
            HoTen = request.HoTen,
            NgaySinh = request.NgaySinh,
            GioiTinh = request.GioiTinh,
            DiaChi = request.DiaChi,
            TaiKhoanId = request.TaiKhoanId
        };

        var result = await _unitOfWork.HocViens.AddAsync(hocVien);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<HocVien?> UpdateAsync(string id, HocVienUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var hocVien = await _unitOfWork.HocViens.GetByIdAsync(id);
        if (hocVien == null)
            throw new KeyNotFoundException($"Không tìm thấy học viên với ID: {id}");

        hocVien.HoTen = request.HoTen;
        hocVien.NgaySinh = request.NgaySinh;
        hocVien.GioiTinh = request.GioiTinh;
        hocVien.DiaChi = request.DiaChi;
        hocVien.TrangThai = request.TrangThai;

        await _unitOfWork.SaveChangesAsync();
        return hocVien;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var hocVien = await _unitOfWork.HocViens.GetByIdAsync(id);
        if (hocVien == null)
            return false;

        await _unitOfWork.HocViens.DeleteAsync(hocVien);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<HocVien?> GetByAccountIdAsync(string accountId)
    {
        if (!Guid.TryParse(accountId, out var id))
            return null;

        var hocViens = await _unitOfWork.HocViens.GetAllAsync(hv => hv.TaiKhoanId == id);
        return hocViens.FirstOrDefault();
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.HocViens.CountAsync();
    }
}
