using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.DTOs.Learning.PhanHoi;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu,hocvien")]

public class PhanHoiController : BaseController<PhanHoi>
{
    private readonly IFeedbackService _service;
    
    public PhanHoiController(IFeedbackService service, ISystemLoggerService logService) : base(logService)
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
            message = "Lấy danh sách phản hồi thành công",
            count = totalItems,
            data = entities
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var phanHoi = await _service.GetByIdAsync(id);
        if (phanHoi == null) return NotFound(new 
        { 
            success = false, 
            message = "Không tìm thấy phản hồi" 
        });

        return Ok(new
        {
            success = true,
            message = "Lấy phản hồi thành công",
            data = phanHoi
        });
    }

    [HttpGet("hocvien/{hocVienId}")]
    public async Task<IActionResult> GetByStudentId(string hocVienId)
    {
        var phanHois = await _service.GetByStudentIdAsync(hocVienId);
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách phản hồi thành công",
            count = phanHois.Count(),
            data = phanHois
        });
    }                   

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PhanHoiRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ",
                errors = ModelState
            });
        }
        var createdPhanHoi = await _service.CreateAsync(request);
        if (createdPhanHoi == null)
        {
            return BadRequest(new 
            { 
                success = false, 
                message = "Tạo phản hồi thất bại" 
            });
        }

        await LogCreateAsync(createdPhanHoi);

        return Ok(new
        {
            success = true,
            message = "Tạo phản hồi thành công",
            data = createdPhanHoi
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingPhanHoi = await _service.GetByIdAsync(id);
        if (existingPhanHoi == null)
        {
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy phản hồi" 
            });
        }

        await LogDeleteAsync(existingPhanHoi);
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return BadRequest(new 
            { 
                success = false, 
                message = "Không thể xóa phản hồi" 
            });
        }

        return Ok(new 
        { 
            success = true, 
            message = "Xóa phản hồi thành công" 
        });
    }
}

