using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Services.Learning;

public class ExamSessionService : IExamSessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<KyThiRequest> _createValidator;
    private readonly IValidator<KyThiUpdateRequest> _updateValidator;

    public ExamSessionService(
        IUnitOfWork unitOfWork,
        IValidator<KyThiRequest> createValidator,
        IValidator<KyThiUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<KyThi?> GetByIdAsync(string id)
    {
        return await _unitOfWork.KyThis.GetByIdAsync(id);
    }

    public async Task<IEnumerable<KyThi>> GetAllAsync()
    {
        return await _unitOfWork.KyThis.GetAllAsync();
    }

    public async Task<KyThi> CreateAsync(KyThiRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var kyThi = new KyThi
        {
            TenKyThi = request.TenKyThi,
            NgayThi = request.NgayThi,
            GioBatDau = request.GioBatDau,
            GioKetThuc = request.GioKetThuc,
            ThoiLuong = request.ThoiLuong,
            LopHocId = request.LopHocId,
            TrangThai = TrangThaiKyThi.sapdienra
        };

        var result = await _unitOfWork.KyThis.AddAsync(kyThi);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<KyThi?> UpdateAsync(string id, KyThiUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var kyThi = await _unitOfWork.KyThis.GetByIdAsync(id);
        if (kyThi == null)
            throw new KeyNotFoundException($"Không tìm thấy kỳ thi với ID: {id}");

        kyThi.TenKyThi = request.TenKyThi;
        kyThi.NgayThi = request.NgayThi;
        kyThi.GioBatDau = request.GioBatDau;
        kyThi.GioKetThuc = request.GioKetThuc;
        kyThi.ThoiLuong = request.ThoiLuong;
        kyThi.TrangThai = request.TrangThai;

        await _unitOfWork.SaveChangesAsync();
        return kyThi;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var kyThi = await _unitOfWork.KyThis.GetByIdAsync(id);
        if (kyThi == null)
            return false;

        await _unitOfWork.KyThis.DeleteAsync(kyThi);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<KyThi>> GetByClassIdAsync(string classId)
    {
        if (!Guid.TryParse(classId, out var id))
            return Enumerable.Empty<KyThi>();

        return await _unitOfWork.KyThis.GetAllAsync(k => k.LopHocId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.KyThis.CountAsync();
    }
}
