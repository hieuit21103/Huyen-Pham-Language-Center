using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Application.DTOs.Courses.LichHoc;

namespace MsHuyenLC.Application.Interfaces.Services.Course;

public interface IScheduleService
{
    Task<LichHoc?> GetByIdAsync(string id);
    Task<IEnumerable<LichHoc>> GetAllAsync();
    Task<IEnumerable<LichHoc>> GetByClassIdAsync(string classId);
    Task<IEnumerable<LichHoc>> GetTeacherSchedulesAsync(string teacherId);
    Task<IEnumerable<LichHoc>> GetStudentSchedulesAsync(string studentId);
    Task<LichHoc> CreateAsync(LichHocRequest request);
    Task<LichHoc?> UpdateAsync(string id, LichHocUpdateRequest request);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<IEnumerable<PhongHoc>> GetAvailableRoomAsync();
    Task<bool> IsRoomAvailable(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, DateOnly startDate, DateOnly endDate);
}
