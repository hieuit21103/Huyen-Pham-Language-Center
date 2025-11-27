using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.User;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Users.HocVien;
using MsHuyenLC.Domain.Entities.Users;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller.Users;

[Route("api/[controller]")]
[ApiController]
public class HocVienController : BaseController
{
    private readonly IStudentService _service;

    public HocVienController(IStudentService service, ISystemLoggerService logService)
        : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var hocViens = await _service.GetAllAsync();
        return Ok(new
        {
            success = true,
            data = hocViens
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetById(string id)
    {
        var hocVien = await _service.GetByIdAsync(id);
        if (hocVien == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy học viên" 
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin học viên thành công",
            data = hocVien
        });
    }

    [Authorize]
    [HttpGet("taikhoan/{id}")]
    public async Task<IActionResult> GetByAccountId(string id)
    {
        var hocVien = await _service.GetByAccountIdAsync(id);
        if (hocVien == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy học viên" 
            });

        return Ok(new
        {
            success = true,
            data = hocVien
        });
    }

    [Authorize(Roles = "admin,giaovu,giaovien,hocvien")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] HocVienUpdateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var hocVien = await _service.GetByIdAsync(id);
        if (hocVien == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy học viên" 
            });

        if(hocVien.TaiKhoanId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value &&
           !User.IsInRole("admin") && !User.IsInRole("giaovu"))
        {
            return Forbid();
        }

        var updatedStudent = await _service.UpdateAsync(id, request);
        if (updatedStudent == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy học viên" 
            });

        await LogUpdateAsync(hocVien, updatedStudent);

        return Ok(new 
        { 
            success = true, 
            message = "Cập nhật học viên thành công",
            data = updatedStudent
        });
    }

    [HttpGet("class/{classId}")]
    [Authorize(Roles = "admin,giaovu,giaovien")]
    public async Task<IActionResult> GetStudentsByClassId(string classId)
    {
        var students = await _service.GetStudentsByClassIdAsync(classId);

        var response = students.Select(s => new
        {
            s.Id,
            s.HoTen,
            s.NgaySinh,
            s.GioiTinh,
            s.DiaChi,
            s.TrangThai,
            s.TaiKhoanId,
            TaiKhoan = s.TaiKhoan != null ? new
            {
                s.TaiKhoan.Email,
                s.TaiKhoan.Sdt
            } : null
        });

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách học viên trong lớp thành công",
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
                message = "Không tìm thấy học viên" 
            });
        
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return BadRequest(new 
            { 
                success = false, 
                message = "Không thể xóa học viên" 
            });

        await LogDeleteAsync(entity);

        return Ok(new 
        { 
            success = true, 
            message = "Xóa học viên thành công" 
        });
    }
}


