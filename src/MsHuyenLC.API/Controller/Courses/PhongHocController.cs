using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.PhongHoc;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.Course;

namespace MsHuyenLC.API.Controller.Courses;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class PhongHocController : BaseController<PhongHoc>
{
    private readonly IRoomService _service;
    public PhongHocController(
        IRoomService service,
        ISystemLoggerService logService) : base(logService)
    {
        _service = service; 
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var phongHocs = await _service.GetAllAsync();

        var response = phongHocs.Select(ph => new PhongHocResponse
        {
            Id = ph.Id,
            TenPhong = ph.TenPhong,
            SoGhe = ph.SoGhe
        });

        var totalItems = await _service.CountAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách phòng học thành công",
            count = totalItems,
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var phongHoc = await _service.GetByIdAsync(id);
        if (phongHoc == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy phòng học" 
            });

        var response = new PhongHocResponse
        {
            Id = phongHoc.Id,
            TenPhong = phongHoc.TenPhong,
            SoGhe = phongHoc.SoGhe
        };

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin phòng học thành công",
            data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PhongHocRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ",
                errors = ModelState
            });

        var result = await _service.CreateAsync(request);
        if (result == null) return BadRequest(new
        {
            success = false,
            message = "Tạo phòng học thất bại"
        });

        await LogCreateAsync(result);
        return Ok(new
        {
            success = true,
            message = "Tạo phòng học thành công",
            data = result
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] PhongHocRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(new
        {
            success = false,
            message = "Dữ liệu không hợp lệ",
            errors = ModelState
        });

        var existingRoom = await _service.GetByIdAsync(id);
        if (existingRoom == null) return NotFound();

        var oldData = new PhongHoc
        {
            Id = existingRoom.Id,
            TenPhong = existingRoom.TenPhong,
            SoGhe = existingRoom.SoGhe
        };

        await _service.UpdateAsync(id, request);

        await LogUpdateAsync(oldData, existingRoom);
        return Ok(new
        {
            success = true,
            message = "Cập nhật phòng học thành công",
            data = existingRoom
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingRoom = await _service.GetByIdAsync(id);
        if (existingRoom == null) return NotFound();

        await _service.DeleteAsync(id);
        await LogDeleteAsync(existingRoom);
        return Ok(new
        {
            success = true,
            message = "Xóa phòng học thành công"
        });
    }
}


