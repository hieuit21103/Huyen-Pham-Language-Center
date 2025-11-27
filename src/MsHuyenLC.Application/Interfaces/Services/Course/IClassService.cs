using MsHuyenLC.Application.DTOs.Courses.LopHoc;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Interfaces.Services.Course;

public interface IClassService
{
    Task<LopHoc?> GetByIdAsync(string id);
    Task<IEnumerable<LopHoc>> GetAllAsync();
    Task<LopHoc> CreateAsync(LopHocRequest request);
    Task<LopHoc?> UpdateAsync(string id, LopHocUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<IEnumerable<LopHoc>> GetByCourseIdAsync(string courseId);
}
