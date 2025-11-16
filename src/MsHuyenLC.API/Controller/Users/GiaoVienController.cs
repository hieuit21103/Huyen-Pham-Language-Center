using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Users.GiaoVien;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.Auth;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.User;

namespace MsHuyenLC.API.Controller.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class GiaoVienController : BaseController
{
    private readonly ITeacherService _service;
    public GiaoVienController(
        ITeacherService service,
        ISystemLoggerService logService) : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entities = await _service.GetAllAsync();
        var totalItems = await _service.CountAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách giáo viên thành công",
            count = totalItems,
            data = entities
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy giáo viên"
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin giáo viên thành công",
            data = entity
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GiaoVienRequest request)
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
                message = "Tạo giáo viên thất bại" 
            });

        var response = new GiaoVienResponse
        {
            Id = result.Id,
            HoTen = result.HoTen,
            ChuyenMon = result.ChuyenMon,
            TrinhDo = result.TrinhDo,
            KinhNghiem = result.KinhNghiem,
            TaiKhoanId = result.TaiKhoanId,
            Email = result.TaiKhoan?.Email,
            Sdt = result.TaiKhoan?.Sdt
        };

        return Ok(new
        {
            success = true,
            message = "Tạo giáo viên thành công",
            data = response
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] GiaoVienUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var result = await _service.UpdateAsync(id, request);
        if (result == null)
            return BadRequest(new 
            { 
                success = false, 
                message = "Cập nhật thông tin giáo viên thất bại" 
            });

        return Ok(new 
        { 
            success = true, 
            message = "Cập nhật thông tin giáo viên thành công",
            data = result
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var giaoVien = await _service.GetByIdAsync(id);

        if (giaoVien == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo viên" 
            });

        await _service.DeleteAsync(id);

        return Ok(new 
        { 
            success = true, 
            message = "Vô hiệu hóa giáo viên thành công" 
        });
    }
}


