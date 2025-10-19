using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.LichHoc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Services.Courses;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]

public class LichHocController : BaseController<LichHoc>
{
    protected readonly IGenericService<PhongHoc> _phongHocService;
    protected readonly IGenericService<LopHoc> _lopHocService;
    protected readonly ScheduleService _ScheduleService;
    public LichHocController(
        IGenericService<LichHoc> service,
        ISystemLoggerService logService,
        ScheduleService ScheduleService,
        IGenericService<PhongHoc> phongHocService,
        IGenericService<LopHoc> lopHocService) : base(service, logService)
    {
        _phongHocService = phongHocService;
        _lopHocService = lopHocService;
        _ScheduleService = ScheduleService;
    }
    protected override Func<IQueryable<LichHoc>, IOrderedQueryable<LichHoc>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "ngayhoc" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(l => l.Thu))
                : (q => q.OrderBy(l => l.Thu)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(l => l.Id))
                : (q => q.OrderBy(l => l.Id)),
        };
    }

    [HttpGet("class/{classId}")]
    [Authorize(Roles="admin,giaovu")]
    public async Task<IActionResult> GetClassSchedule(
            string classId, 
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
        )
    {
        var schedule = await _service.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
                Filter: s => s.LopHocId.ToString() == classId,
            OrderBy: q => q.OrderBy(s => s.Thu)
        );

        return Ok(schedule);
    }

    [HttpGet("teacher/{teacherId}")]
    [Authorize(Roles="admin,giaovu,giaovien")]
    public async Task<IActionResult> GetTeacherSchedule(
            string teacherId, 
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var schedule = await _service.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
                Filter: s => s.LopHocId != null && s.LopHocId.ToString() == teacherId,
            OrderBy: q => q.OrderBy(s => s.Thu)
        );

        return Ok(schedule);
    }

    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "admin,giaovu,giaovien,hocvien")]
    public async Task<IActionResult> GetStudentSchedule(
            string studentId, 
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var schedule = await _service.GetAllAsync(
            PageNumber: pageNumber,
            PageSize: pageSize,
                Filter: s => s.LopHocId != null && s.LopHocId.ToString() == studentId,
            OrderBy: q => q.OrderBy(s => s.Thu)
        );

        return Ok(schedule);
    }

    [HttpPost]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Create([FromBody] LichHocRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

            var lichHoc = new LichHoc 
        {
                LopHocId = request.LopHocId,
                PhongHocId = request.PhongHocId,
            Thu = request.Thu,
            TuNgay = request.TuNgay,
            DenNgay = request.DenNgay,
            GioBatDau = request.GioBatDau,
            GioKetThuc = request.GioKetThuc,
            CoHieuLuc = true
        };

        var result = await _service.AddAsync(lichHoc);
        if (result == null)
        {
            return BadRequest(new { message = "Không thể tạo lịch học." });
        }

        await LogCreateAsync(result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Update(string id, [FromBody] LichHocUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingLichHoc = await _service.GetByIdAsync(id); 
        if (existingLichHoc == null) return NotFound();

            var phongHoc = await _phongHocService.GetByIdAsync(request.PhongHocId.ToString());
        if (phongHoc == null) return BadRequest("Phòng học không tồn tại.");
            
            var oldData = new LichHoc
            {
                Id = existingLichHoc.Id,
                LopHocId = existingLichHoc.LopHocId,
                PhongHocId = existingLichHoc.PhongHocId,
                Thu = existingLichHoc.Thu,
                TuNgay = existingLichHoc.TuNgay,
                DenNgay = existingLichHoc.DenNgay,
                GioBatDau = existingLichHoc.GioBatDau,
                GioKetThuc = existingLichHoc.GioKetThuc,
                CoHieuLuc = existingLichHoc.CoHieuLuc
            };

            existingLichHoc.PhongHocId = request.PhongHocId;
            existingLichHoc.Thu = request.Thu;
            existingLichHoc.TuNgay = request.TuNgay;
            existingLichHoc.DenNgay = request.DenNgay;
            existingLichHoc.GioBatDau = request.GioBatDau;
            existingLichHoc.GioKetThuc = request.GioKetThuc;
            existingLichHoc.CoHieuLuc = request.CoHieuLuc;

        await _service.UpdateAsync(existingLichHoc);
        await LogUpdateAsync(oldData, existingLichHoc);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingLichHoc = await _service.GetByIdAsync(id);
        if (existingLichHoc == null) return NotFound();

        await _service.DeleteAsync(existingLichHoc);
        await LogDeleteAsync(existingLichHoc);
        return Ok();
    }

    [HttpGet("available-rooms")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAvailableRooms()
    {
        var available = await _ScheduleService.GetAvailableRoomAsync();
        return Ok(available);
    }
}
