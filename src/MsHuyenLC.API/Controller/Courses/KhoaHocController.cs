using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Application.Interfaces.Services.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.KhoaHoc;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
public class KhoaHocController : BaseController<KhoaHoc>
{
    private readonly ICourseService _service;
    public KhoaHocController(ICourseService service, ISystemLoggerService logService) 
        : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var khoaHocs = await _service.GetAllAsync();

        var response = khoaHocs.Select(kh => new KhoaHocResponse
        {
            Id = kh.Id,
            TenKhoaHoc = kh.TenKhoaHoc,
            MoTa = kh.MoTa,
            HocPhi = kh.HocPhi,
            ThoiLuong = kh.ThoiLuong,
            NgayKhaiGiang = kh.NgayKhaiGiang,
            TrangThai = kh.TrangThai
        });

        var totalItems = await _service.CountAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách khóa học thành công",
            count = totalItems,
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var khoaHoc = await _service.GetByIdAsync(id);
        if (khoaHoc == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy khóa học" 
            });

        var response = new KhoaHocResponse
        {
            Id = khoaHoc.Id,
            TenKhoaHoc = khoaHoc.TenKhoaHoc,
            MoTa = khoaHoc.MoTa,
            HocPhi = khoaHoc.HocPhi,
            ThoiLuong = khoaHoc.ThoiLuong,
            NgayKhaiGiang = khoaHoc.NgayKhaiGiang,
            TrangThai = khoaHoc.TrangThai
        };

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin khóa học thành công",
            data = response
        });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] KhoaHocRequest request)
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
                message = "Tạo khóa học thất bại" 
            });

        await LogCreateAsync(result);

        var response = new KhoaHocResponse
        {
            Id = result.Id,
            TenKhoaHoc = result.TenKhoaHoc,
            MoTa = result.MoTa,
            HocPhi = result.HocPhi,
            ThoiLuong = result.ThoiLuong,
            NgayKhaiGiang = result.NgayKhaiGiang,
            TrangThai = result.TrangThai
        };

        return Ok(
            new
            {
                success = true,
                message = "Tạo khóa học thành công",
                data = response
            });
    }   

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] KhoaHocUpdateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var existingKhoaHoc = await _service.GetByIdAsync(id);
        if (existingKhoaHoc == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy khóa học" 
            });

        var oldData = new KhoaHoc
        {
            Id = existingKhoaHoc.Id,
            TenKhoaHoc = existingKhoaHoc.TenKhoaHoc,
            MoTa = existingKhoaHoc.MoTa,
            HocPhi = existingKhoaHoc.HocPhi,
            ThoiLuong = existingKhoaHoc.ThoiLuong,
            NgayKhaiGiang = existingKhoaHoc.NgayKhaiGiang,
            TrangThai = existingKhoaHoc.TrangThai
        };

        await _service.UpdateAsync(id, request);
        await LogUpdateAsync(oldData, existingKhoaHoc);

        return Ok(
            new
            {
                success = true,
                message = "Cập nhật khóa học thành công",
                data = existingKhoaHoc
            }
        );
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
                message = "Không tìm thấy khóa học" 
            });
        
        await _service.DeleteAsync(id);
        await LogDeleteAsync(entity);

        return Ok(
            new
            {
                success = true,
                message = "Xóa khóa học thành công"
            }
        );
    }
}


