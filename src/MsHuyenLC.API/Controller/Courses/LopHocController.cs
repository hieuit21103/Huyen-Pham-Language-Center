using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces.Services.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.LopHoc;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
public class LopHocController : BaseController
{
    private readonly IClassService _service;
    public LopHocController(
        IClassService service,
        ISystemLoggerService logService) : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var lopHocs = await _service.GetAllAsync();

        var response = lopHocs.Select(lh => new LopHocResponse
        {
            Id = lh.Id,
            TenLop = lh.TenLop,
            SiSoToiDa = lh.SiSoToiDa,
            SiSoHienTai = lh.SiSoHienTai,
            TrangThai = lh.TrangThai,
            KhoaHocId = lh.KhoaHocId
        });

        var totalItems = await _service.CountAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách lớp học thành công",
            count = totalItems,
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var lopHoc = await _service.GetByIdAsync(id);
        if (lopHoc == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy lớp học"
            });

        var response = new LopHocResponse
        {
            Id = lopHoc.Id,
            TenLop = lopHoc.TenLop,
            SiSoToiDa = lopHoc.SiSoToiDa,
            SiSoHienTai = lopHoc.SiSoHienTai,
            TrangThai = lopHoc.TrangThai,
            KhoaHocId = lopHoc.KhoaHocId
        };

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin lớp học thành công",
            data = response
        });
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourseId(string courseId)
    {
        var lopHocs = await _service.GetByCourseIdAsync(courseId);

        var response = lopHocs.Select(lh => new LopHocResponse
        {
            Id = lh.Id,
            TenLop = lh.TenLop,
            SiSoToiDa = lh.SiSoToiDa,
            SiSoHienTai = lh.SiSoHienTai,
            TrangThai = lh.TrangThai,
            KhoaHocId = lh.KhoaHocId
        });

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách lớp học theo khóa học thành công",
            data = response
        });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LopHocRequest request)
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
            return BadRequest(new
            {
                success = false,
                message = "Tạo lớp học thất bại"
            });

        await LogCreateAsync(result);

        var response = new LopHocResponse
        {
            Id = result.Id,
            TenLop = result.TenLop,
            SiSoToiDa = result.SiSoToiDa,
            SiSoHienTai = result.SiSoHienTai,
            TrangThai = result.TrangThai,
            KhoaHocId = result.KhoaHocId,
        };

        return Ok(new
        {
            success = true,
            message = "Tạo lớp học thành công",
            data = response
        });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] LopHocUpdateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var existingLopHoc = await _service.GetByIdAsync(id);
        if (existingLopHoc == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lớp học" 
            });

        var oldData = new LopHoc
        {
            Id = existingLopHoc.Id,
            TenLop = existingLopHoc.TenLop,
            SiSoToiDa = existingLopHoc.SiSoToiDa,
            SiSoHienTai = existingLopHoc.SiSoHienTai,
            TrangThai = existingLopHoc.TrangThai,
            KhoaHocId = existingLopHoc.KhoaHocId
        };

        var result = await _service.UpdateAsync(id, request);
        if (result == null)
            return BadRequest(new
            {
                success = false,
                message = "Cập nhật lớp học thất bại"
            });

        await LogUpdateAsync(oldData, result);

        var response = new LopHocResponse
        {
            Id = result.Id,
            TenLop = result.TenLop,
            SiSoToiDa = result.SiSoToiDa,
            SiSoHienTai = result.SiSoHienTai,
            TrangThai = result.TrangThai,
            KhoaHocId = result.KhoaHocId
        };

        return Ok(new
        {
            success = true,
            message = "Cập nhật lớp học thành công",
            data = response
        });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lớp học" 
            });
        
        await LogDeleteAsync(entity);
        
        var result = await _service.DeleteAsync(id);
        if (!result)
            return BadRequest(new
            {
                success = false,
                message = "Xóa lớp học thất bại"
            });

        return Ok(new
        {
            success = true,
            message = "Xóa lớp học thành công"
        });
    }
}


