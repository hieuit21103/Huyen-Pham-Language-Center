using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses;
using System.Linq.Expressions;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
public class KhoaHocController : BaseController<KhoaHoc>
{
    public KhoaHocController(IGenericService<KhoaHoc> service) : base(service)
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
    public async Task<IActionResult> Create([FromBody] KhoaHocCreateRequest createDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var khoaHoc = new KhoaHoc
        {
            TenKhoaHoc = createDto.TenKhoaHoc,
            MoTa = createDto.MoTa,
            HocPhi = createDto.HocPhi,
            ThoiLuong = createDto.ThoiLuong,
            NgayKhaiGiang = createDto.NgayKhaiGiang.ToUniversalTime(),
            TrangThai = createDto.TrangThai
        };

        var result = await _service.AddAsync(khoaHoc);
        if (result == null) return BadRequest();

        return Ok(createDto);
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] KhoaHocUpdateRequest updateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingKhoaHoc = await _service.GetByIdAsync(id);
        if (existingKhoaHoc == null) return NotFound();

        existingKhoaHoc.TenKhoaHoc = updateDto.TenKhoaHoc;
        existingKhoaHoc.MoTa = updateDto.MoTa;
        existingKhoaHoc.HocPhi = updateDto.HocPhi;
        existingKhoaHoc.ThoiLuong = updateDto.ThoiLuong;
        existingKhoaHoc.NgayKhaiGiang = updateDto.NgayKhaiGiang.ToUniversalTime();
        existingKhoaHoc.TrangThai = updateDto.TrangThai;

        await _service.UpdateAsync(existingKhoaHoc);

        return Ok();
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        await _service.DeleteAsync(entity);

        return Ok();
    }
}
