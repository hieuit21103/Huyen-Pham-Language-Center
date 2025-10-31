using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.ThongBao;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Learning;

[Route("api/[controller]")]
[ApiController]
public class ThongBaoController : BaseController<ThongBao>
{
    public ThongBaoController(
        IGenericService<ThongBao> service,
        ISystemLoggerService logService) : base(service, logService)
    {
    }

    protected override Func<IQueryable<ThongBao>, IOrderedQueryable<ThongBao>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "tieude" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(tb => tb.TieuDe))
                : (q => q.OrderBy(tb => tb.TieuDe)),
            "ngaytao" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(tb => tb.NgayTao))
                : (q => q.OrderBy(tb => tb.NgayTao)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(tb => tb.NgayTao))
                : (q => q.OrderBy(tb => tb.NgayTao)),
        };
    }

    [HttpGet]
    [Authorize]
    public override async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "asc"
    )
    {
        var thongBaos = await _service.GetAllAsync(
            pageNumber,
            pageSize,
            Includes: tb => tb.NguoiGui
        );

        var response = thongBaos.Select(tb => new ThongBaoResponse
        {
            Id = tb.Id,
            NguoiGuiId = tb.NguoiGuiId,
            TenNguoiGui = tb.NguoiGui.TenDangNhap,
            TieuDe = tb.TieuDe,
            NoiDung = tb.NoiDung,
            NgayTao = tb.NgayTao
        });

        var totalItems = await _service.CountAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách thông báo thành công",
            count = totalItems,
            data = response
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

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new 
            { 
                success = false, 
                message = "Không xác thực được người dùng" 
            });

        var thongBao = new ThongBao
        {
            TieuDe = request.TieuDe,
            NoiDung = request.NoiDung,
            NguoiGuiId = userId,
            NgayTao = DateTime.UtcNow
        };

        var result = await _service.AddAsync(thongBao);
        if (result == null) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Tạo thông báo thất bại" 
            });

        await LogCreateAsync(result);

        var response = new ThongBaoResponse
        {
            Id = result.Id,
            NguoiGuiId = result.NguoiGuiId,
            TieuDe = result.TieuDe,
            NoiDung = result.NoiDung,
            NgayTao = result.NgayTao
        };

        return Ok(new
        {
            success = true,
            message = "Tạo thông báo thành công",
            data = response
        });
    }

    /// <summary>
    /// Cập nhật thông báo (chỉ người gửi)
    /// </summary>
    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ThongBaoRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

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

        var oldData = new ThongBao
        {
            Id = existingThongBao.Id,
            TieuDe = existingThongBao.TieuDe,
            NoiDung = existingThongBao.NoiDung,
            NguoiGuiId = existingThongBao.NguoiGuiId,
            NgayTao = existingThongBao.NgayTao
        };

        existingThongBao.TieuDe = request.TieuDe;
        existingThongBao.NoiDung = request.NoiDung;

        await _service.UpdateAsync(existingThongBao);
        await LogUpdateAsync(oldData, existingThongBao);

        var response = new ThongBaoResponse
        {
            Id = existingThongBao.Id,
            NguoiGuiId = existingThongBao.NguoiGuiId,
            TieuDe = existingThongBao.TieuDe,
            NoiDung = existingThongBao.NoiDung,
            NgayTao = existingThongBao.NgayTao
        };

        return Ok(new
        {
            success = true,
            message = "Cập nhật thông báo thành công",
            data = response
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

        // Kiểm tra quyền: chỉ người gửi mới được xóa
        if (existingThongBao.NguoiGuiId != userId)
            return Forbid();

        await LogDeleteAsync(existingThongBao);
        await _service.DeleteAsync(existingThongBao);

        return Ok(new 
        { 
            success = true, 
            message = "Xóa thông báo thành công" 
        });
    }
}
