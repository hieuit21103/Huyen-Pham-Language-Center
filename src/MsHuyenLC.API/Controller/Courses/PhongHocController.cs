using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.PhongHoc;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Courses;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class PhongHocController : BaseController<PhongHoc>
{
    public PhongHocController(
        IGenericService<PhongHoc> service,
        ISystemLoggerService logService) : base(service, logService)
    {
    }

    protected override Func<IQueryable<PhongHoc>, IOrderedQueryable<PhongHoc>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "tenphong" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.TenPhong))
                : (q => q.OrderBy(k => k.TenPhong)),
            "soghe" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.SoGhe))
                : (q => q.OrderBy(k => k.SoGhe)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
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

        var phongHoc = new PhongHoc
        {
            TenPhong = request.TenPhong,
            SoGhe = request.SoGhe
        };

        var result = await _service.AddAsync(phongHoc);
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

        existingRoom.TenPhong = request.TenPhong ?? existingRoom.TenPhong;
        existingRoom.SoGhe = request.SoGhe != 0 ? request.SoGhe : existingRoom.SoGhe;

        await _service.UpdateAsync(existingRoom);

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

        await _service.DeleteAsync(existingRoom);
        await LogDeleteAsync(existingRoom);

        return Ok(new
        {
            success = true,
            message = "Xóa phòng học thành công"
        });
    }
}
