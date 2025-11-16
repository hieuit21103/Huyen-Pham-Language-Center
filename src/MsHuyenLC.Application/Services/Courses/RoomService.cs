using FluentValidation;
using MsHuyenLC.Application.DTOs.Courses.PhongHoc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Services.Courses;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PhongHocRequest> _createValidator;

    public RoomService(
        IUnitOfWork unitOfWork,
        IValidator<PhongHocRequest> createValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<PhongHoc?> GetByIdAsync(string id)
    {
        return await _unitOfWork.PhongHocs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PhongHoc>> GetAllAsync()
    {
        return await _unitOfWork.PhongHocs.GetAllAsync();
    }

    public async Task<PhongHoc> CreateAsync(PhongHocRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var phongHoc = new PhongHoc
        {
            TenPhong = request.TenPhong,
            SoGhe = request.SoGhe
        };

        var result = await _unitOfWork.PhongHocs.AddAsync(phongHoc);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<PhongHoc?> UpdateAsync(string id, PhongHocRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var existingRoom = await _unitOfWork.PhongHocs.GetByIdAsync(id);
        if (existingRoom == null)
            return null;

        existingRoom.TenPhong = request.TenPhong ?? existingRoom.TenPhong;
        existingRoom.SoGhe = request.SoGhe != 0 ? request.SoGhe : existingRoom.SoGhe;

        await _unitOfWork.PhongHocs.UpdateAsync(existingRoom);
        await _unitOfWork.SaveChangesAsync();
        return existingRoom;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var phongHoc = await _unitOfWork.PhongHocs.GetByIdAsync(id);
        if (phongHoc == null)
            return false;

        await _unitOfWork.PhongHocs.DeleteAsync(phongHoc);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.PhongHocs.CountAsync();
    }

    public async Task<IEnumerable<PhongHoc>> GetAvailableRoomsAsync(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        var allRooms = await _unitOfWork.PhongHocs.GetAllAsync();
        
        var activeLichHocs = await _unitOfWork.LichHocs.GetAllAsync(lh => lh.CoHieuLuc);
        
        var occupiedRoomIds = new HashSet<Guid>();
        foreach (var lichHoc in activeLichHocs)
        {
            var conflictingThoiGianBieus = await _unitOfWork.ThoiGianBieus.GetAllAsync(
                tgb => tgb.LichHocId == lichHoc.Id &&
                       tgb.Thu == dayOfWeek &&
                       tgb.GioBatDau < endTime &&
                       tgb.GioKetThuc > startTime
            );
            
            if (conflictingThoiGianBieus.Any())
            {
                occupiedRoomIds.Add(lichHoc.PhongHocId);
            }
        }

        return allRooms.Where(room => !occupiedRoomIds.Contains(room.Id));
    }
}
