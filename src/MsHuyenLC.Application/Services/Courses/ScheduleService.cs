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

    public async Task<LichHocResponse?> GetByIdWithDetailsAsync(string id)
    {
        var lichHoc = await _unitOfWork.LichHocs.GetByIdAsync(id);
        if (lichHoc == null)
            return null;

        var thoiGianBieus = await _unitOfWork.ThoiGianBieus.GetAllAsync(tgb => tgb.LichHocId == lichHoc.Id);

        return new LichHocResponse
        {
            Id = lichHoc.Id,
            LopHocId = lichHoc.LopHocId,
            PhongHocId = lichHoc.PhongHocId,
            TuNgay = lichHoc.TuNgay,
            DenNgay = lichHoc.DenNgay,
            CoHieuLuc = lichHoc.CoHieuLuc,
            ThoiGianBieu = thoiGianBieus.Select(tgb => new ThoiGianBieuResponse
            {
                Id = tgb.Id,
                Thu = tgb.Thu,
                GioBatDau = tgb.GioBatDau,
                GioKetThuc = tgb.GioKetThuc
            }).ToList()
        };
    }

    public async Task<IEnumerable<LichHocResponse>> GetAllWithDetailsAsync()
    {
        var lichHocs = await _unitOfWork.LichHocs.GetAllAsync();
        var result = new List<LichHocResponse>();

        foreach (var lh in lichHocs)
        {
            var thoiGianBieus = await _unitOfWork.ThoiGianBieus.GetAllAsync(tgb => tgb.LichHocId == lh.Id);
            
            result.Add(new LichHocResponse
            {
                Id = lh.Id,
                LopHocId = lh.LopHocId,
                PhongHocId = lh.PhongHocId,
                TuNgay = lh.TuNgay,
                DenNgay = lh.DenNgay,
                CoHieuLuc = lh.CoHieuLuc,
                ThoiGianBieu = thoiGianBieus.Select(tgb => new ThoiGianBieuResponse
                {
                    Id = tgb.Id,
                    Thu = tgb.Thu,
                    GioBatDau = tgb.GioBatDau,
                    GioKetThuc = tgb.GioKetThuc
                }).ToList()
            });
        }

        return result;
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

        var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(request.LopHocId.ToString());
        if (lopHoc == null)
            throw new KeyNotFoundException("Lớp học không tồn tại");

        var tuNgay = lopHoc.KhoaHoc.NgayKhaiGiang;
        var soBuoiKhoaHoc = lopHoc.KhoaHoc.ThoiLuong;

        var lichHoc = new LichHoc
        {
            LopHocId = request.LopHocId,
            PhongHocId = request.PhongHocId,
            TuNgay = tuNgay,
            DenNgay = tuNgay,
            CoHieuLuc = true
        };

        var result = await _unitOfWork.LichHocs.AddAsync(lichHoc);
        
        if (request.ThoiGianBieus != null && request.ThoiGianBieus.Any())
        {
            foreach (var tgb in request.ThoiGianBieus)
            {
                var thoiGianBieu = new ThoiGianBieu
                {
                    LichHocId = lichHoc.Id,
                    Thu = tgb.Thu,
                    GioBatDau = tgb.GioBatDau,
                    GioKetThuc = tgb.GioKetThuc
                };
                await _unitOfWork.ThoiGianBieus.AddAsync(thoiGianBieu);
            }

            lichHoc.DenNgay = CalculateDenNgay(tuNgay, soBuoiKhoaHoc, request.ThoiGianBieus.Count);
        }

        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<LichHoc?> UpdateAsync(string id, LichHocUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var lichHoc = await _unitOfWork.LichHocs.GetByIdAsync(id);
        if (lichHoc == null)
            return null;

        var lopHoc = await _unitOfWork.LopHocs.GetByIdAsync(lichHoc.LopHocId.ToString());
        if (lopHoc == null)
            throw new KeyNotFoundException("Lớp học không tồn tại");

        lichHoc.PhongHocId = request.PhongHocId;
        lichHoc.CoHieuLuc = request.CoHieuLuc;

        var oldThoiGianBieus = await _unitOfWork.ThoiGianBieus.GetAllAsync(
            filter: tgb => tgb.LichHocId == lichHoc.Id
        );
        foreach (var tgb in oldThoiGianBieus)
        {
            await _unitOfWork.ThoiGianBieus.DeleteAsync(tgb);
        }

        if (request.ThoiGianBieus != null && request.ThoiGianBieus.Any())
        {
            foreach (var tgb in request.ThoiGianBieus)
            {
                var thoiGianBieu = new ThoiGianBieu
                {
                    LichHocId = lichHoc.Id,
                    Thu = tgb.Thu,
                    GioBatDau = tgb.GioBatDau,
                    GioKetThuc = tgb.GioKetThuc
                };
                await _unitOfWork.ThoiGianBieus.AddAsync(thoiGianBieu);
            }

            var soBuoiKhoaHoc = lopHoc.KhoaHoc.ThoiLuong;
            lichHoc.DenNgay = CalculateDenNgay(lichHoc.TuNgay, soBuoiKhoaHoc, request.ThoiGianBieus.Count);
        }

        await _unitOfWork.SaveChangesAsync();
        return lichHoc;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var lichHoc = await _unitOfWork.LichHocs.GetByIdAsync(id);
        if (lichHoc == null)
            return false;

        var thoiGianBieus = await _unitOfWork.ThoiGianBieus.GetAllAsync(
            filter: tgb => tgb.LichHocId == lichHoc.Id
        );
        foreach (var tgb in thoiGianBieus)
        {
            await _unitOfWork.ThoiGianBieus.DeleteAsync(tgb);
        }

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
            var thoiGianBieus = await _unitOfWork.ThoiGianBieus.GetAllAsync(
                filter: tgb => tgb.LichHocId == lichHoc.Id
            );

            foreach (var tgb in thoiGianBieus)
            {
                if (await IsRoomAvailable(tgb.Thu, tgb.GioBatDau, tgb.GioKetThuc, lichHoc.TuNgay, lichHoc.DenNgay))
                {
                    availableRooms.Add(lichHoc.PhongHoc);
                    break;
                }
            }
        }

        return availableRooms.Distinct();
    }

    public async Task<bool> IsRoomAvailable(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, DateOnly startDate, DateOnly endDate)
    {
        var conflictingLichHocs = await _unitOfWork.LichHocs.GetAllAsync(
            filter: lh => lh.CoHieuLuc &&
                    lh.TuNgay < endDate &&
                    lh.DenNgay > startDate
        );

        foreach (var lichHoc in conflictingLichHocs)
        {
            var thoiGianBieus = await _unitOfWork.ThoiGianBieus.GetAllAsync(
                filter: tgb => tgb.LichHocId == lichHoc.Id &&
                              tgb.Thu == dayOfWeek &&
                              tgb.GioBatDau < endTime &&
                              tgb.GioKetThuc > startTime
            );

            if (thoiGianBieus.Any())
                return false; 
        }

        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.LichHocs.CountAsync();
    }

    private DateOnly CalculateDenNgay(DateOnly tuNgay, int soBuoiKhoaHoc, int soThoiGianBieuTrongTuan)
    {
        if (soThoiGianBieuTrongTuan <= 0)
            throw new ArgumentException("Số thời gian biểu trong tuần phải lớn hơn 0");

        int soTuan = (int)Math.Ceiling((double)soBuoiKhoaHoc / soThoiGianBieuTrongTuan);
        
        int soNgay = soTuan * 7;

        if (soNgay > 3650)
            soNgay = 3650;

        return tuNgay.AddDays(soNgay - 1);
    }
}

