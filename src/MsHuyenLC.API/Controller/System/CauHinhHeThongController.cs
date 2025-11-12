using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.System.CauHinhHeThong;

namespace MsHuyenLC.API.Controller.System;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
public class CauHinhHeThongController : BaseController<CauHinhHeThong>
{
    private readonly ISystemConfigService _service;
    public CauHinhHeThongController(
        ISystemConfigService service, 
        ISystemLoggerService logService) 
        : base(logService)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy cấu hình theo tên
    /// </summary>
    [HttpGet("by-name/{name}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _service.GetByNameAsync(name);
        if (result == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy cấu hình" 
            });

        return Ok(new
        {
            success = true,
            message = "Lấy cấu hình thành công",
            data = new CauHinhHeThongResponse
            {
                Id = result.Id,
                Ten = result.Ten,
                GiaTri = result.GiaTri
            }
        });
    }

    /// <summary>
    /// Tạo cấu hình mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CauHinhHeThongRequest request)
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
                message = "Tạo cấu hình thất bại" 
            });

        await LogCreateAsync(result);

        return Ok(new 
        { 
            success = true, 
            message = "Tạo cấu hình thành công",
            data = new CauHinhHeThongResponse
            {
                Id = result.Id,
                Ten = result.Ten,
                GiaTri = result.GiaTri
            }
        });
    }

    /// <summary>
    /// Cập nhật cấu hình
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CauHinhHeThongUpdateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var cauHinh = await _service.GetByIdAsync(id);
        if (cauHinh == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy cấu hình" 
            });

        var oldData = new CauHinhHeThong
        {
            Id = cauHinh.Id,
            Ten = cauHinh.Ten,
            GiaTri = cauHinh.GiaTri
        };

        await _service.UpdateAsync(id, request);
        await LogUpdateAsync(oldData, cauHinh);

        return Ok(new 
        { 
            success = true, 
            message = "Cập nhật cấu hình thành công",
            data = new CauHinhHeThongResponse
            {
                Id = cauHinh.Id,
                Ten = cauHinh.Ten,
                GiaTri = cauHinh.GiaTri
            }
        });
    }

    /// <summary>
    /// Xóa cấu hình
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy cấu hình" 
            });
        
        await _service.DeleteAsync(id);
        await LogDeleteAsync(entity);

        return Ok(new 
        { 
            success = true, 
            message = "Xóa cấu hình thành công" 
        });
    }
}


