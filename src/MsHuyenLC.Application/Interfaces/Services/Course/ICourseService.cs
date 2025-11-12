using MsHuyenLC.Application.DTOs.Courses.KhoaHoc;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Interfaces.Services.Course;

public interface ICourseService
{
    Task<KhoaHoc?> GetByIdAsync(string id);
    Task<IEnumerable<KhoaHoc>> GetAllAsync();
    Task<KhoaHoc> CreateAsync(KhoaHocRequest request);
    Task<KhoaHoc?> UpdateAsync(string id, KhoaHocUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
}
