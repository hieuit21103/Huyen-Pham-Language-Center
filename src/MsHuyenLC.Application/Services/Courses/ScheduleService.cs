using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Services.Courses;

public class ScheduleService : GenericService<LichHoc>
{
    public ScheduleService(IGenericRepository<LichHoc> repository)
        : base(repository)
    {
    }

    public async Task<IEnumerable<PhongHoc>> GetAvailableRoomAsync()
    {
        var lichHocs = await _repository.GetAllAsync(
            1,
            int.MaxValue,
            Filter: lh => lh.CoHieuLuc
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

    private async Task<bool> IsRoomAvailable(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, DateTime startDate, DateTime endDate)
    {
        var conflictingSchedules = await _repository.GetAllAsync(
            1,
            int.MaxValue,
            Filter: lh => lh.CoHieuLuc &&
                    lh.Thu == dayOfWeek &&
                    lh.GioBatDau < endTime &&
                    lh.GioKetThuc > startTime &&
                    lh.TuNgay < endDate &&
                    lh.DenNgay > startDate
        );

        return !conflictingSchedules.Any();
    }
}