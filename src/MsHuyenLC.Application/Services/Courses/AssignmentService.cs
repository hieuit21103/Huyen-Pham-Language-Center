using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Application.DTOs.Courses.PhanCong;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Services.Courses;

public class AssignmentService : IAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public AssignmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PhanCong?> GetByIdAsync(string id)
    {
        return await _unitOfWork.PhanCongs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PhanCong>> GetAllAsync()
    {
        return await _unitOfWork.PhanCongs.GetAllAsync();
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

    public async Task<IEnumerable<PhanCong>> GetAllByTeacherIdAsync(string teacherId)
    {
        if (!Guid.TryParse(teacherId, out var id))
            return Enumerable.Empty<PhanCong>();

        return await _unitOfWork.PhanCongs.GetAllAsync(
            filter: p => p.GiaoVienId == id
        );
    }

    public async Task<IEnumerable<PhanCong>> GetAllByClassIdAsync(string classId)
    {
        if (!Guid.TryParse(classId, out var id))
            return Enumerable.Empty<PhanCong>();

        return await _unitOfWork.PhanCongs.GetAllAsync(
            filter: p => p.LopHocId == id
        );
    }

    public async Task<PhanCong> CreateAsync(PhanCongRequest request)
    {
        var phanCong = new PhanCong
        {
            GiaoVienId = request.GiaoVienId,
            LopHocId = request.LopHocId,
            NgayPhanCong = DateTime.UtcNow
        };

        var result = await _unitOfWork.PhanCongs.AddAsync(phanCong);
        await _unitOfWork.SaveChangesAsync();
        return result;
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
}

