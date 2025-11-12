using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.ThongBao;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.Learning;

namespace MsHuyenLC.API.Controller.Learning;

[Route("api/[controller]")]
[ApiController]
public class ThongBaoController : BaseController<ThongBao>
{
    private readonly INotificationService _service;
    
    public ThongBaoController(
        INotificationService service,
        ISystemLoggerService logService) : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var thongBaos = await _service.GetAllAsync();
        var totalItems = await _service.CountAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách thông báo thành công",
            count = totalItems,
            data = thongBaos
        });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(string id)
    {
        var thongBao = await _service.GetByIdAsync(id);
        if (thongBao == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông báo"
            });

        return Ok(new
        {
            success = true,
            data = thongBao
        });
    }

    [HttpGet("nguoi-gui/{nguoiGuiId}")]
    [Authorize]
    public async Task<IActionResult> GetBySender(string nguoiGuiId)
    {
        var thongBaos = await _service.GetBySenderIdAsync(nguoiGuiId);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách thông báo thành công",
            count = thongBaos.Count(),
            data = thongBaos
        });
    }

    [HttpGet("nguoi-nhan/{nguoiNhanId}")]
    [Authorize]
    public async Task<IActionResult> GetByReceiver(string nguoiNhanId)
    {
        var thongBaos = await _service.GetByReceiverIdAsync(nguoiNhanId);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách thông báo thành công",
            count = thongBaos.Count(),
            data = thongBaos
        });
    }

    /// <summary>
    /// Tạo thông báo mới (Admin/GiaoVu)
    /// </summary>
    [Authorize(Roles = "admin,giaovu")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ThongBaoRequest request)
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
                message = "Tạo thông báo thất bại" 
            });

        await LogCreateAsync(result);

        return Ok(new
        {
            success = true,
            message = "Tạo thông báo thành công",
            data = result
        });
    }

    /// <summary>
    /// Xóa thông báo (chỉ người gửi)
    /// </summary>
    [Authorize(Roles = "admin,giaovu")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new 
            { 
                success = false, 
                message = "Không xác thực được người dùng" 
            });

        var existingThongBao = await _service.GetByIdAsync(id);
        if (existingThongBao == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy thông báo" 
            });

        if (existingThongBao.NguoiGuiId != userId)
            return Forbid();

        await LogDeleteAsync(existingThongBao);
        var deleted = await _service.DeleteAsync(id);
        
        if (!deleted)
            return BadRequest(new 
            { 
                success = false, 
                message = "Không thể xóa thông báo" 
            });

        return Ok(new 
        { 
            success = true, 
            message = "Xóa thông báo thành công" 
        });
    }
}


