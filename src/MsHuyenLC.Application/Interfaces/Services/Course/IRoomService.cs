using MsHuyenLC.Application.DTOs.Courses.PhongHoc;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Interfaces.Services.Course;

public interface IRoomService
{
    Task<PhongHoc?> GetByIdAsync(string id);
    Task<IEnumerable<PhongHoc>> GetAllAsync();
    Task<PhongHoc> CreateAsync(PhongHocRequest request);
    Task<PhongHoc?> UpdateAsync(string id, PhongHocRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<IEnumerable<PhongHoc>> GetAvailableRoomsAsync(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime);
}
