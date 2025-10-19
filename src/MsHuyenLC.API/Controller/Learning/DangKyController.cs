using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.DangKy;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Learning;

[Route("api/[controller]")]
[ApiController]
public class DangKyController : BaseController<DangKy>
{
    public DangKyController(IGenericService<DangKy> service, ISystemLoggerService logService) 
        : base(service, logService)
    {
    }

    protected override Func<IQueryable<DangKy>, IOrderedQueryable<DangKy>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "ngaydangky" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.NgayDangKy))
                : (q => q.OrderBy(k => k.NgayDangKy)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [HttpPost("student")]
    [Authorize]
    public async Task<IActionResult> RegisterStudent([FromBody] DangKyRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var dangKy = new DangKy
        {
            KhoaHocId = request.KhoaHocId,
            HocVienId = request.HocVienId,
            TrangThai = 0,
        };

        var result = await _service.AddAsync(dangKy);
        if (result == null) return BadRequest();

        await LogCreateAsync(result);

        var response = new DangKyResponse
        {
            Id = result.Id,
            HocVienId = result.HocVienId,
            LopHocId = result.LopHocId,
            NgayDangKy = result.NgayDangKy
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> CreateDangKy([FromBody] DangKyCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var dangKy = new DangKy
        {
            NgayDangKy = request.NgayDangKy ?? DateTime.UtcNow,
            LopHocId = request.LopHocId ?? null,
            NgayXepLop = request.NgayXepLop ?? null,
            KhoaHocId = request.KhoaHocId,
            HocVienId = request.HocVienId,
            TrangThai = 0,
        };

        var result = await _service.AddAsync(dangKy);
        if (result == null) return BadRequest();

        await LogCreateAsync(result);

        var response = new DangKyResponse
        {
            Id = result.Id,
            HocVienId = result.HocVienId,
            LopHocId = result.LopHocId,
            NgayDangKy = result.NgayDangKy
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> UpdateDangKy(string id, [FromBody] DangKyUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingDangKy = await _service.GetByIdAsync(id);
        if (existingDangKy == null) return NotFound();

        var oldData = new DangKy
        {
            Id = existingDangKy.Id,
            LopHocId = existingDangKy.LopHocId,
            NgayXepLop = existingDangKy.NgayXepLop,
            TrangThai = existingDangKy.TrangThai,
            NgayDangKy = existingDangKy.NgayDangKy,
            KhoaHocId = existingDangKy.KhoaHocId,
            HocVienId = existingDangKy.HocVienId
        };

        existingDangKy.LopHocId = request.LopHocId ?? existingDangKy.LopHocId;
        existingDangKy.NgayXepLop = request.NgayXepLop ?? existingDangKy.NgayXepLop;
        existingDangKy.TrangThai = request.TrangThai ?? existingDangKy.TrangThai;
        existingDangKy.NgayDangKy = request.NgayDangKy ?? existingDangKy.NgayDangKy;
        existingDangKy.KhoaHocId = request.KhoaHocId ?? existingDangKy.KhoaHocId;
        existingDangKy.HocVienId = request.HocVienId ?? existingDangKy.HocVienId;

        await _service.UpdateAsync(existingDangKy);

        await LogUpdateAsync(oldData, existingDangKy);

        var response = new DangKyResponse
        {
            Id = existingDangKy.Id,
            HocVienId = existingDangKy.HocVienId,
            LopHocId = existingDangKy.LopHocId,
            NgayDangKy = existingDangKy.NgayDangKy,
            NgayXepLop = existingDangKy.NgayXepLop,
            KhoaHocId = existingDangKy.KhoaHocId,
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> DeleteDangKy(string id)
    {
        var existingDangKy = await _service.GetByIdAsync(id);
        if (existingDangKy == null) return NotFound();

        await _service.DeleteAsync(existingDangKy);

        await LogDeleteAsync(existingDangKy);

        return Ok();
    }
}
