using FluentValidation;
using MsHuyenLC.Application.DTOs.System.CauHinhHeThong;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.System;

namespace MsHuyenLC.Application.Services.System;

public class SystemConfigService : ISystemConfigService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CauHinhHeThongRequest> _createValidator;
    private readonly IValidator<CauHinhHeThongUpdateRequest> _updateValidator;

    public SystemConfigService(
        IUnitOfWork unitOfWork,
        IValidator<CauHinhHeThongRequest> createValidator,
        IValidator<CauHinhHeThongUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<CauHinhHeThong?> GetByIdAsync(string id)
    {
        return await _unitOfWork.CauHinhHeThongs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<CauHinhHeThong>> GetAllAsync()
    {
        return await _unitOfWork.CauHinhHeThongs.GetAllAsync();
    }

    public async Task<CauHinhHeThong> CreateAsync(CauHinhHeThongRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var cauHinh = new CauHinhHeThong
        {
            Ten = request.Ten,
            GiaTri = request.GiaTri
        };

        var result = await _unitOfWork.CauHinhHeThongs.AddAsync(cauHinh);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<CauHinhHeThong?> UpdateAsync(string id, CauHinhHeThongUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var cauHinh = await _unitOfWork.CauHinhHeThongs.GetByIdAsync(id);
        if (cauHinh == null)
            throw new KeyNotFoundException($"Không tìm thấy cấu hình hệ thống với ID: {id}");

        cauHinh.Ten = request.Ten;
        cauHinh.GiaTri = request.GiaTri;

        await _unitOfWork.SaveChangesAsync();
        return cauHinh;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var cauHinh = await _unitOfWork.CauHinhHeThongs.GetByIdAsync(id);
        if (cauHinh == null)
            return false;

        await _unitOfWork.CauHinhHeThongs.DeleteAsync(cauHinh);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<CauHinhHeThong?> GetByNameAsync(string name)
    {
        var configs = await _unitOfWork.CauHinhHeThongs.GetAllAsync(c => c.Ten == name);
        return configs.FirstOrDefault();
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.CauHinhHeThongs.CountAsync();
    }
}
