using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.KhoaHoc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Services.Courses;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<KhoaHocRequest> _createValidator;
    private readonly IValidator<KhoaHocUpdateRequest> _updateValidator;

    public CourseService(
        IUnitOfWork unitOfWork,
        IValidator<KhoaHocRequest> createValidator,
        IValidator<KhoaHocUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<KhoaHoc?> GetByIdAsync(string id)
    {
        return await _unitOfWork.KhoaHocs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<KhoaHoc>> GetAllAsync()
    {
        return await _unitOfWork.KhoaHocs.GetAllAsync();
    }

    public async Task<KhoaHoc> CreateAsync(KhoaHocRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var khoaHoc = new KhoaHoc
        {
            TenKhoaHoc = request.TenKhoaHoc,
            MoTa = request.MoTa,
            HocPhi = request.HocPhi,
            ThoiLuong = request.ThoiLuong,
            NgayKhaiGiang = request.NgayKhaiGiang
        };

        var result = await _unitOfWork.KhoaHocs.AddAsync(khoaHoc);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<KhoaHoc?> UpdateAsync(string id, KhoaHocUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var khoaHoc = await _unitOfWork.KhoaHocs.GetByIdAsync(id);
        if (khoaHoc == null)
            throw new KeyNotFoundException($"Không tìm thấy khóa học với ID: {id}");

        khoaHoc.TenKhoaHoc = request.TenKhoaHoc;
        khoaHoc.MoTa = request.MoTa;
        khoaHoc.HocPhi = request.HocPhi;
        khoaHoc.ThoiLuong = request.ThoiLuong;
        khoaHoc.NgayKhaiGiang = request.NgayKhaiGiang;
        khoaHoc.TrangThai = request.TrangThai;

        await _unitOfWork.SaveChangesAsync();
        return khoaHoc;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var khoaHoc = await _unitOfWork.KhoaHocs.GetByIdAsync(id);
        if (khoaHoc == null)
            return false;

        await _unitOfWork.KhoaHocs.DeleteAsync(khoaHoc);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.KhoaHocs.CountAsync();
    }
}
