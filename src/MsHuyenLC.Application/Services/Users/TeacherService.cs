using FluentValidation;
using MsHuyenLC.Application.DTOs.Users.GiaoVien;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Services.Users;

public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GiaoVienRequest> _createValidator;
    private readonly IValidator<GiaoVienUpdateRequest> _updateValidator;

    public TeacherService(
        IUnitOfWork unitOfWork,
        IValidator<GiaoVienRequest> createValidator,
        IValidator<GiaoVienUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<GiaoVien?> GetByIdAsync(string id)
    {
        return await _unitOfWork.GiaoViens.GetByIdAsync(id);
    }

    public async Task<IEnumerable<GiaoVien>> GetAllAsync()
    {
        return await _unitOfWork.GiaoViens.GetAllAsync();
    }

    public async Task<GiaoVien> CreateAsync(GiaoVienRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var giaoVien = new GiaoVien
        {
            HoTen = request.HoTen,
            ChuyenMon = request.ChuyenMon,
            TrinhDo = request.TrinhDo,
            KinhNghiem = request.KinhNghiem,
            TaiKhoanId = request.TaiKhoanId
        };

        var result = await _unitOfWork.GiaoViens.AddAsync(giaoVien);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<GiaoVien?> UpdateAsync(string id, GiaoVienUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var giaoVien = await _unitOfWork.GiaoViens.GetByIdAsync(id);
        if (giaoVien == null)
            throw new KeyNotFoundException($"Không tìm thấy giáo viên với ID: {id}");

        giaoVien.HoTen = request.HoTen;
        giaoVien.ChuyenMon = request.ChuyenMon;
        giaoVien.TrinhDo = request.TrinhDo;
        giaoVien.KinhNghiem = request.KinhNghiem;

        await _unitOfWork.SaveChangesAsync();
        return giaoVien;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var giaoVien = await _unitOfWork.GiaoViens.GetByIdAsync(id);
        if (giaoVien == null)
            return false;
        var taiKhoan = await _unitOfWork.TaiKhoans.GetByIdAsync(giaoVien.TaiKhoanId.ToString());
        if (taiKhoan != null)
            await _unitOfWork.TaiKhoans.DeleteAsync(taiKhoan);
        await _unitOfWork.GiaoViens.DeleteAsync(giaoVien);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<GiaoVien?> GetByAccountIdAsync(string accountId)
    {
        if (!Guid.TryParse(accountId, out var id))
            return null;

        var giaoViens = await _unitOfWork.GiaoViens.GetAllAsync(gv => gv.TaiKhoanId == id);
        return giaoViens.FirstOrDefault();
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.GiaoViens.CountAsync();
    }
}
