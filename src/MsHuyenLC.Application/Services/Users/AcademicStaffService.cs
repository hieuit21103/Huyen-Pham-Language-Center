using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.GiaoVu;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Services.User;

public class AcademicStaffService : IAcademicStaffService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GiaoVuRequest> _validator;
    private readonly IValidator<GiaoVuUpdateRequest> _updateValidator;

    public AcademicStaffService(
        IUnitOfWork unitOfWork,
        IValidator<GiaoVuRequest> validator,
        IValidator<GiaoVuUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _updateValidator = updateValidator;
    }

    public async Task<GiaoVu?> GetByIdAsync(string id)
    {
        return await _unitOfWork.GiaoVus.GetByIdAsync(id);
    }

    public async Task<IEnumerable<GiaoVu>> GetAllAsync()
    {
        return await _unitOfWork.GiaoVus.GetAllAsync();
    }

    public async Task<GiaoVu?> CreateAsync(GiaoVuRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);

        var giaoVu = new GiaoVu
        {
            HoTen = request.HoTen,
            BoPhan = request.BoPhan,
            TaiKhoanId = request.TaiKhoanId
        };

        var result = await _unitOfWork.GiaoVus.AddAsync(giaoVu);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<GiaoVu?> UpdateAsync(string id, GiaoVuUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var giaoVu = await _unitOfWork.GiaoVus.GetByIdAsync(id);
        if (giaoVu == null)
            throw new KeyNotFoundException($"Không tìm thấy giáo vụ với ID: {id}");

        giaoVu.HoTen = request.HoTen ?? giaoVu.HoTen;
        giaoVu.BoPhan = request.BoPhan ?? giaoVu.BoPhan;
        giaoVu.TaiKhoanId = request.TaiKhoanId ?? giaoVu.TaiKhoanId;

        await _unitOfWork.GiaoVus.UpdateAsync(giaoVu);
        await _unitOfWork.SaveChangesAsync();
        return giaoVu;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var giaoVu = await _unitOfWork.GiaoVus.GetByIdAsync(id);
        if (giaoVu == null)
            return false;

        await _unitOfWork.GiaoVus.DeleteAsync(giaoVu);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<GiaoVu?> GetByAccountIdAsync(string accountId)
    {
        if (!Guid.TryParse(accountId, out var id))
            return null;

        var giaoVus = await _unitOfWork.GiaoVus.GetAllAsync(gv => gv.TaiKhoanId == id);
        return giaoVus.FirstOrDefault();
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.GiaoVus.CountAsync();
    }
}
