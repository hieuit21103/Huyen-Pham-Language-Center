using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Users.GiaoVu;
using System.Security.Claims;
using MsHuyenLC.Application.Interfaces.Services.User;

namespace MsHuyenLC.API.Controller.Users;

[Route("api/[controller]")]
[ApiController]
public class GiaoVuController : BaseController<GiaoVu>
{
    private readonly IAcademicStaffService _service;
    public GiaoVuController(IAcademicStaffService service, ISystemLoggerService logService)
        : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var entities = await _service.GetAllAsync();
        var totalItems = await _service.CountAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách giáo vụ thành công",
            count = totalItems,
            data = entities
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo vụ" 
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin giáo vụ thành công",
            data = result
        });
    }

    [HttpGet("taikhoan/{id}")]
    public async Task<IActionResult> GetByAccountId(string id)
    {
        var result = await _service.GetByAccountIdAsync(id);
        if (result == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo vụ" 
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin giáo vụ thành công",
            data = result
        });
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GiaoVuRequest request)
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
                message = "Tạo giáo vụ thất bại" 
            });

        await LogCreateAsync(result);

        return Ok(new 
        { 
            success = true, 
            message = "Tạo giáo vụ thành công",
            data = result
        });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] GiaoVuUpdateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var giaoVu = await _service.GetByIdAsync(id);
        if (giaoVu == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo vụ" 
            });

        // Chỉ cho phép admin hoặc chính giáo vụ đó cập nhật
        if(giaoVu.TaiKhoanId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value &&
           !User.IsInRole("admin"))
        {
            return Forbid();
        }

        var oldData = new GiaoVu
        {
            Id = giaoVu.Id,
            HoTen = giaoVu.HoTen,
            BoPhan = giaoVu.BoPhan,
            TaiKhoanId = giaoVu.TaiKhoanId
        };

        await _service.UpdateAsync(id, request);
        await LogUpdateAsync(oldData, giaoVu);

        return Ok(new 
        { 
            success = true, 
            message = "Cập nhật giáo vụ thành công",
            data = giaoVu
        });
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo vụ" 
            });
        
        await _service.DeleteAsync(id);
        await LogDeleteAsync(entity);

        return Ok(new 
        { 
            success = true, 
            message = "Xóa giáo vụ thành công" 
        });
    }
}


