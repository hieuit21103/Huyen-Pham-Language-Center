using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Users.GiaoVu;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller.Users;

[Route("api/[controller]")]
[ApiController]
public class GiaoVuController : BaseController<GiaoVu>
{
    public GiaoVuController(IGenericService<GiaoVu> service, ISystemLoggerService logService) 
        : base(service, logService)
    {
    }

    protected override Func<IQueryable<GiaoVu>, IOrderedQueryable<GiaoVu>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "hoten" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.HoTen))
                : (q => q.OrderBy(k => k.HoTen)),
            "bophan" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.BoPhan))
                : (q => q.OrderBy(k => k.BoPhan)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [Authorize]
    [HttpGet("taikhoan/{id}")]
    public async Task<IActionResult> GetByAccountId(string id)
    {
        var result = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: 1,
            Filter: gv => gv.TaiKhoanId.ToString() == id
        );

        var giaoVu = result.FirstOrDefault();
        if (giaoVu == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo vụ" 
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin giáo vụ thành công",
            data = giaoVu
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

        var giaoVu = new GiaoVu
        {
            HoTen = request.HoTen,
            BoPhan = request.BoPhan,
            TaiKhoanId = request.TaiKhoanId
        };

        var result = await _service.AddAsync(giaoVu);
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
    public async Task<IActionResult> Update(string id, [FromBody] GiaoVuRequest request)
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

        giaoVu.HoTen = request.HoTen;
        giaoVu.BoPhan = request.BoPhan;
        giaoVu.TaiKhoanId = request.TaiKhoanId;

        await _service.UpdateAsync(giaoVu);
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
        
        await _service.DeleteAsync(entity);
        await LogDeleteAsync(entity);

        return Ok(new 
        { 
            success = true, 
            message = "Xóa giáo vụ thành công" 
        });
    }
}
