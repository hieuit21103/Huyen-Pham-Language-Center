using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.KhoaHoc;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
public class KhoaHocController : BaseController<KhoaHoc>
{
    public KhoaHocController(IGenericService<KhoaHoc> service, ISystemLoggerService logService) 
        : base(service, logService)
    {
    }

    protected override Func<IQueryable<KhoaHoc>, IOrderedQueryable<KhoaHoc>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "tenkhoahoc" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.TenKhoaHoc))
                : (q => q.OrderBy(k => k.TenKhoaHoc)),
            "hocphi" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.HocPhi))
                : (q => q.OrderBy(k => k.HocPhi)),
            "ngaykhaigiang" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.NgayKhaiGiang))
                : (q => q.OrderBy(k => k.NgayKhaiGiang)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] KhoaHocRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var khoaHoc = new KhoaHoc
        {
            TenKhoaHoc = request.TenKhoaHoc,
            MoTa = request.MoTa,
            HocPhi = request.HocPhi,
            ThoiLuong = request.ThoiLuong,
            NgayKhaiGiang = request.NgayKhaiGiang
        };

        var result = await _service.AddAsync(khoaHoc);
        if (result == null) return BadRequest();

        await LogCreateAsync(result);

        var response = new KhoaHocResponse
        {
            Id = result.Id,
            TenKhoaHoc = result.TenKhoaHoc,
            MoTa = result.MoTa,
            HocPhi = result.HocPhi,
            ThoiLuong = result.ThoiLuong,
            NgayKhaiGiang = result.NgayKhaiGiang,
            TrangThai = result.TrangThai
        };

        return Ok(response);
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] KhoaHocUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingKhoaHoc = await _service.GetByIdAsync(id);
        if (existingKhoaHoc == null) return NotFound();

        var oldData = new KhoaHoc
        {
            Id = existingKhoaHoc.Id,
            TenKhoaHoc = existingKhoaHoc.TenKhoaHoc,
            MoTa = existingKhoaHoc.MoTa,
            HocPhi = existingKhoaHoc.HocPhi,
            ThoiLuong = existingKhoaHoc.ThoiLuong,
            NgayKhaiGiang = existingKhoaHoc.NgayKhaiGiang,
            TrangThai = existingKhoaHoc.TrangThai
        };

        existingKhoaHoc.TenKhoaHoc = request.TenKhoaHoc;
        existingKhoaHoc.MoTa = request.MoTa;
        existingKhoaHoc.HocPhi = request.HocPhi;
        existingKhoaHoc.ThoiLuong = request.ThoiLuong;
        existingKhoaHoc.NgayKhaiGiang = request.NgayKhaiGiang;
        existingKhoaHoc.TrangThai = request.TrangThai;

        await _service.UpdateAsync(existingKhoaHoc);

        await LogUpdateAsync(oldData, existingKhoaHoc);

        return Ok();
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        
        await _service.DeleteAsync(entity);

        await LogDeleteAsync(entity);

        return Ok();
    }
}
