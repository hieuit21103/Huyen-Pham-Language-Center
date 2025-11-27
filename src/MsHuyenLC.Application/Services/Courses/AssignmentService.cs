using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Application.DTOs.Courses.PhanCong;
using MsHuyenLC.Domain.Entities.Courses;
using FluentValidation;
using MsHuyenLC.Application.Interfaces.Services.System;

namespace MsHuyenLC.Application.Services.Courses;

public class AssignmentService : IAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PhanCongRequest> _validator;
    private readonly IValidator<PhanCongUpdateRequest> _updateValidator;

    public AssignmentService(
        IUnitOfWork unitOfWork,
        IValidator<PhanCongRequest> validator,
        IValidator<PhanCongUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _updateValidator = updateValidator;
    }

    public async Task<PhanCong?> GetByIdAsync(string id)
    {
        var result = await _unitOfWork.PhanCongs.GetByIdAsync(id);
        if (result == null)
            return null;
        return result;
        
    }

    public async Task<IEnumerable<PhanCong?>> GetAllAsync()
    {
        var results = await _unitOfWork.PhanCongs.GetAllAsync();
        return results;
    }

    public async Task<PhanCong?> GetByTeacherIdAsync(string teacherId)
    {
        var results = await _unitOfWork.PhanCongs.GetAllAsync(
            filter: p => p.GiaoVien.Id.ToString() == teacherId
        );
        return results.FirstOrDefault();
    }

    public async Task<PhanCong?> GetByClassIdAsync(string classId)
    {
        var results = await _unitOfWork.PhanCongs.GetAllAsync(
            filter: p => p.LopHoc.Id.ToString() == classId
        );
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<PhanCong?>> GetAllByTeacherIdAsync(string teacherId)
    {
        if (!Guid.TryParse(teacherId, out var id))
            return Enumerable.Empty<PhanCong>();

        var results = await _unitOfWork.PhanCongs.GetAllAsync(
            filter: p => p.GiaoVienId == id
        ); 

        return results;
    }

    public async Task<IEnumerable<PhanCong?>> GetAllByClassIdAsync(string classId)
    {
        if (!Guid.TryParse(classId, out var id))
            return Enumerable.Empty<PhanCong>();

        var result = await _unitOfWork.PhanCongs.GetAllAsync(
            filter: p => p.LopHocId == id
        );

        return result;
    }

    public async Task<PhanCong?> AssignTeacher(string id, PhanCongUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var phanCong = await _unitOfWork.PhanCongs.GetByIdAsync(id);
        if (phanCong == null)
            return null;

        phanCong.GiaoVienId = request.GiaoVienId ?? phanCong.GiaoVienId;

        await _unitOfWork.SaveChangesAsync();
        return phanCong;
    }

    public async Task<PhanCong?> CreateAsync(PhanCongRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var phanCong = new PhanCong
        {
            GiaoVienId = request.GiaoVienId,
            LopHocId = request.LopHocId,
            NgayPhanCong = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var result = await _unitOfWork.PhanCongs.AddAsync(phanCong);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<PhanCong?> UpdateAsync(string id, PhanCongUpdateRequest request)
    {
        var phanCong = await _unitOfWork.PhanCongs.GetByIdAsync(id);
        if (phanCong == null)
            return null;

        phanCong.GiaoVienId = request.GiaoVienId ?? phanCong.GiaoVienId;
        phanCong.LopHocId = request.LopHocId ?? phanCong.LopHocId;

        await _unitOfWork.SaveChangesAsync();
        return phanCong;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var phanCong = await _unitOfWork.PhanCongs.GetByIdAsync(id);
        if (phanCong == null)
            return false;

        await _unitOfWork.PhanCongs.DeleteAsync(phanCong);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.PhanCongs.CountAsync();
    }

    public async Task<PhanCongResponse?> MapToResponse(PhanCong? phanCong)
    {
        if (phanCong == null) return null;

        var giaoVien = await _unitOfWork.GiaoViens.GetByIdAsync(phanCong.GiaoVienId.ToString());
        var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(phanCong.LopHocId.ToString());

        return new PhanCongResponse
        {
            Id = phanCong.Id,
            GiaoVienId = phanCong.GiaoVienId,
            GiaoVien = giaoVien,
            LopHocId = phanCong.LopHocId,
            LopHoc = lopHoc,
            NgayPhanCong = phanCong.NgayPhanCong
        };
    }
}

