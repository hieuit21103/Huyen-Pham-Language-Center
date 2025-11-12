using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Application.DTOs.Courses.LichHoc;
using FluentValidation;

namespace MsHuyenLC.Application.Services.Courses;

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<LichHocRequest> _createValidator;
    private readonly IValidator<LichHocUpdateRequest> _updateValidator;
    public ScheduleService(
        IUnitOfWork unitOfWork,
        IValidator<LichHocRequest> createValidator,
        IValidator<LichHocUpdateRequest> updateValidator
    )
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<LichHoc?> GetByIdAsync(string id)
    {
        return await _unitOfWork.LichHocs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<LichHoc>> GetByClassIdAsync(string classId)
    {
        return await _unitOfWork.LichHocs.GetAllAsync(
            filter: lh => lh.LopHocId.ToString() == classId
        );
    }

    public async Task<IEnumerable<LichHoc>> GetTeacherSchedulesAsync(string teacherId)
    {
        var assignments = await _unitOfWork.PhanCongs.GetAllAsync(
            filter: pc => pc.GiaoVienId.ToString() == teacherId,
            includes: pc => pc.LopHoc
        );

        var schedules = new List<LichHoc>();

        foreach (var assignment in assignments)
        {
            var classSchedules = await _unitOfWork.LichHocs.GetAllAsync(
                filter: lh => lh.LopHocId == assignment.LopHocId
            );
            schedules.AddRange(classSchedules);
        }

        return schedules;
    }

    public async Task<IEnumerable<LichHoc>> GetStudentSchedulesAsync(string studentId)
    {
        var enrollments = await _unitOfWork.DangKyKhoaHocs.GetAllAsync(
            filter: gd => gd.HocVienId.ToString() == studentId,
            includes: gd => gd.LopHoc!
        );

        var schedules = new List<LichHoc>();

        foreach (var enrollment in enrollments)
        {
            var classSchedules = await _unitOfWork.LichHocs.GetAllAsync(
                filter: lh => lh.LopHocId == enrollment.LopHocId
            );
            schedules.AddRange(classSchedules);
        }

        return schedules;
    }

    public async Task<IEnumerable<LichHoc>> GetAllAsync()
    {
        return await _unitOfWork.LichHocs.GetAllAsync();
    }

    public async Task<LichHoc> CreateAsync(LichHocRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var lichHoc = new LichHoc
        {
            LopHocId = request.LopHocId,
            PhongHocId = request.PhongHocId,
            TuNgay = request.TuNgay,
            DenNgay = request.DenNgay,
            GioBatDau = request.GioBatDau,
            GioKetThuc = request.GioKetThuc,
            Thu = request.Thu
        };

        var result = await _unitOfWork.LichHocs.AddAsync(lichHoc);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<LichHoc?> UpdateAsync(string id, LichHocUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var lichHoc = await _unitOfWork.LichHocs.GetByIdAsync(id);
        if (lichHoc == null)
            return null;

        lichHoc.PhongHocId = request.PhongHocId;
        lichHoc.TuNgay = request.TuNgay;
        lichHoc.DenNgay = request.DenNgay;
        lichHoc.GioBatDau = request.GioBatDau;
        lichHoc.GioKetThuc = request.GioKetThuc;
        lichHoc.Thu = request.Thu;
        lichHoc.CoHieuLuc = request.CoHieuLuc;

        await _unitOfWork.SaveChangesAsync();
        return lichHoc;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var lichHoc = await _unitOfWork.LichHocs.GetByIdAsync(id);
        if (lichHoc == null)
            return false;

        await _unitOfWork.LichHocs.DeleteAsync(lichHoc);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PhongHoc>> GetAvailableRoomAsync()
    {
        var lichHocs = await _unitOfWork.LichHocs.GetAllAsync(
            filter: lh => lh.CoHieuLuc
        );

        var availableRooms = new List<PhongHoc>();

        foreach (var lichHoc in lichHocs)
        {
            var dayOfWeek = lichHoc.Thu;
            var startTime = lichHoc.GioBatDau;
            var endTime = lichHoc.GioKetThuc;
            var startDate = lichHoc.TuNgay;
            var endDate = lichHoc.DenNgay;
            if (await IsRoomAvailable(dayOfWeek, startTime, endTime, startDate, endDate))
            {
                availableRooms.Add(lichHoc.PhongHoc);
            }
        }

        return availableRooms;
    }

    public async Task<bool> IsRoomAvailable(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, DateOnly startDate, DateOnly endDate)
    {
        var conflictingSchedules = await _unitOfWork.LichHocs.GetAllAsync(
            filter: lh => lh.CoHieuLuc &&
                    lh.Thu == dayOfWeek &&
                    lh.GioBatDau < endTime &&
                    lh.GioKetThuc > startTime &&
                    lh.TuNgay < endDate &&
                    lh.DenNgay > startDate
        );

        return !conflictingSchedules.Any();
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.LichHocs.CountAsync();
    }
}

