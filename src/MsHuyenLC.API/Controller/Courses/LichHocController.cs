using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.LichHoc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Application.Interfaces.Services.System;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]

public class LichHocController : BaseController
{
    private readonly IScheduleService _service;
    public LichHocController(
        ISystemLoggerService logService,
        IScheduleService service) : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var lichHocs = await _service.GetAllAsync();

        var response = lichHocs.Select(lh => new LichHocResponse
        {
            Id = lh.Id,
            LopHocId = lh.LopHocId,
            PhongHocId = lh.PhongHocId,
            Thu = lh.Thu,
            TuNgay = lh.TuNgay,
            DenNgay = lh.DenNgay,
            GioBatDau = lh.GioBatDau,
            GioKetThuc = lh.GioKetThuc,
            CoHieuLuc = lh.CoHieuLuc
        });

        var totalItems = await _service.CountAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách lịch học thành công",
            count = totalItems,
            data = response
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetById(string id)
    {
        var lichHoc = await _service.GetByIdAsync(id);
        if (lichHoc == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy lịch học"
            });

        var response = new LichHocResponse
        {
            Id = lichHoc.Id,
            LopHocId = lichHoc.LopHocId,
            PhongHocId = lichHoc.PhongHocId,
            Thu = lichHoc.Thu,
            TuNgay = lichHoc.TuNgay,
            DenNgay = lichHoc.DenNgay,
            GioBatDau = lichHoc.GioBatDau,
            GioKetThuc = lichHoc.GioKetThuc,
            CoHieuLuc = lichHoc.CoHieuLuc
        };
        return Ok(new
        {
            success = true,
            message = "Lấy thông tin lịch học thành công",
            data = response
        });
    }

    [HttpGet("class/{classId}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetClassSchedule(string classId)
    {
        var schedule = await _service.GetByClassIdAsync(classId);

        return Ok(new{
            success = true,
            message = "Lấy lịch học của lớp thành công",
            data = schedule
        });
    }

    [HttpGet("teacher/{teacherId}")]
    [Authorize(Roles = "admin,giaovu,giaovien")]
    public async Task<IActionResult> GetTeacherSchedule(string teacherId)
    {
        var schedule = await _service.GetTeacherSchedulesAsync(teacherId);

        if (!schedule.Any())
        {
            return Ok(new
            {
                success = true,
                message = "Giáo viên không có phân công giảng dạy",
                data = schedule
            });
        }

        return Ok(new{
            success = true,
            message = "Lấy lịch học của giáo viên thành công",
            data = schedule
        });
    }

    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "admin,giaovu,giaovien,hocvien")]
    public async Task<IActionResult> GetStudentSchedule(string studentId)
    {
        var schedule = await _service.GetStudentSchedulesAsync(studentId);
        return Ok(new
        {
            success = true,
            message = "Lấy lịch học của học viên thành công",
            data = schedule
        });
    }

    [HttpPost]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Create([FromBody] LichHocRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var result = await _service.CreateAsync(request);
        if (result == null)
        {
            return BadRequest(new 
            { 
                success = false, 
                message = "Tạo lịch học thất bại" 
            });
        }

        await LogCreateAsync(result);

        return Ok(new
        {
            success = true,
            message = "Tạo lịch học thành công",
            data = result
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Update(string id, [FromBody] LichHocUpdateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var existingLichHoc = await _service.GetByIdAsync(id);
        if (existingLichHoc == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lịch học" 
            });

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

        await _service.UpdateAsync(id, request);
        await LogUpdateAsync(oldData, existingLichHoc);
        
        return Ok(new
        {
            success = true,
            message = "Cập nhật lịch học thành công",
            data = existingLichHoc
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingLichHoc = await _service.GetByIdAsync(id);
        if (existingLichHoc == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lịch học" 
            });

        await _service.DeleteAsync(id);
        await LogDeleteAsync(existingLichHoc);
        
        return Ok(new
        {
            success = true,
            message = "Xóa lịch học thành công"
        });
    }

    [HttpGet("available-rooms")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAvailableRooms()
    {
        var available = await _service.GetAvailableRoomAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy phòng học trống thành công",
            data = available
        });
    }
}


