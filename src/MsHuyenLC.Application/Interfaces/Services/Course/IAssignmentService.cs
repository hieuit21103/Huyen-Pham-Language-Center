using MsHuyenLC.Application.DTOs.Courses.PhanCong;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Interfaces.Services.Course;

public interface IAssignmentService
{
    Task<PhanCong?> GetByIdAsync(string id);
    Task<IEnumerable<PhanCong?>> GetAllAsync();
    Task<PhanCong?> GetByTeacherIdAsync(string teacherId);
    Task<PhanCong?> GetByClassIdAsync(string classId);
    Task<IEnumerable<PhanCong?>> GetAllByTeacherIdAsync(string teacherId);
    Task<IEnumerable<PhanCong?>> GetAllByClassIdAsync(string classId);
    Task<PhanCong?> AssignTeacher(string id, PhanCongUpdateRequest request);
    Task<PhanCong?> CreateAsync(PhanCongRequest request);
    Task<PhanCong?> UpdateAsync(string id, PhanCongUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
}
