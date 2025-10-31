using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.System.CauHinhHeThong;

namespace MsHuyenLC.API.Controller.System;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
public class CauHinhHeThongController : BaseController<CauHinhHeThong>
{
    public CauHinhHeThongController(
        IGenericService<CauHinhHeThong> service, 
        ISystemLoggerService logService) 
        : base(service, logService)
    {
    }

    protected override Func<IQueryable<CauHinhHeThong>, IOrderedQueryable<CauHinhHeThong>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "ten" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Ten))
                : (q => q.OrderBy(k => k.Ten)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    /// <summary>
    /// Lấy cấu hình theo tên
    /// </summary>
    [HttpGet("by-name/{ten}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByName(string ten)
    {
        var result = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: 1,
            Filter: ch => ch.Ten == ten
        );

        var cauHinh = result.FirstOrDefault();
        if (cauHinh == null) 
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
                Id = cauHinh.Id,
                Ten = cauHinh.Ten,
                GiaTri = cauHinh.GiaTri
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

        // Kiểm tra tên cấu hình đã tồn tại chưa
        var existing = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: 1,
            Filter: ch => ch.Ten == request.Ten
        );

        if (existing.Any())
            return Conflict(new 
            { 
                success = false, 
                message = "Tên cấu hình đã tồn tại" 
            });

        var cauHinh = new CauHinhHeThong
        {
            Ten = request.Ten,
            GiaTri = request.GiaTri
        };

        var result = await _service.AddAsync(cauHinh);
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

        // Kiểm tra tên mới có trùng không (nếu đổi tên)
        if (!string.IsNullOrWhiteSpace(request.Ten) && request.Ten != cauHinh.Ten)
        {
            var existing = await _service.GetAllAsync(
                PageNumber: 1,
                PageSize: 1,
                Filter: ch => ch.Ten == request.Ten
            );

            if (existing.Any())
                return Conflict(new 
                { 
                    success = false, 
                    message = "Tên cấu hình đã tồn tại" 
                });
        }

        var oldData = new CauHinhHeThong
        {
            Id = cauHinh.Id,
            Ten = cauHinh.Ten,
            GiaTri = cauHinh.GiaTri
        };

        if (!string.IsNullOrWhiteSpace(request.Ten))
            cauHinh.Ten = request.Ten;
        
        if (!string.IsNullOrWhiteSpace(request.GiaTri))
            cauHinh.GiaTri = request.GiaTri;

        await _service.UpdateAsync(cauHinh);
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
        
        await _service.DeleteAsync(entity);
        await LogDeleteAsync(entity);

        return Ok(new 
        { 
            success = true, 
            message = "Xóa cấu hình thành công" 
        });
    }
}
