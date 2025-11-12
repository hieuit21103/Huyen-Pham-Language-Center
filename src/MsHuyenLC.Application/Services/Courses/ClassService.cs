using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.LopHoc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Services.Courses;

public class ClassService : IClassService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<LopHocRequest> _createValidator;
    private readonly IValidator<LopHocUpdateRequest> _updateValidator;

    public ClassService(
        IUnitOfWork unitOfWork,
        IValidator<LopHocRequest> createValidator,
        IValidator<LopHocUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<LopHoc?> GetByIdAsync(string id)
    {
        return await _unitOfWork.LopHocs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<LopHoc>> GetAllAsync()
    {
        return await _unitOfWork.LopHocs.GetAllAsync();
    }

    public async Task<LopHoc> CreateAsync(LopHocRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var lopHoc = new LopHoc
        {
            TenLop = request.TenLop,
            KhoaHocId = request.KhoaHocId,
            SiSoToiDa = request.SiSoToiDa,
            SiSoHienTai = 0
        };

        var result = await _unitOfWork.LopHocs.AddAsync(lopHoc);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<LopHoc?> UpdateAsync(string id, LopHocUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(id);
        if (lopHoc == null)
            throw new KeyNotFoundException($"Không tìm thấy lớp học với ID: {id}");

        lopHoc.SiSoToiDa = request.SiSoToiDa;
        lopHoc.TrangThai = request.TrangThai;

        await _unitOfWork.SaveChangesAsync();
        return lopHoc;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(id);
        if (lopHoc == null)
            return false;

        await _unitOfWork.LopHocs.DeleteAsync(lopHoc);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.LopHocs.CountAsync();
    }

    public async Task<IEnumerable<LopHoc>> GetByCourseIdAsync(string courseId)
    {
        if (!Guid.TryParse(courseId, out var id))
            return Enumerable.Empty<LopHoc>();

        return await _unitOfWork.LopHocs.GetAllAsync(x => x.KhoaHocId == id);
    }
}
