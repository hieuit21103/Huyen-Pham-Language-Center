using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Classes;
using System.Linq.Expressions;
using MsHuyenLC.Application.Services;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
public class LopHocController : BaseController<LopHoc>
{
    protected readonly IGenericService<KhoaHoc> _courseService;
    public LopHocController(IGenericService<LopHoc> service, IGenericService<KhoaHoc> courseService) : base(service)
    {
        _courseService = courseService;
    }

    protected override Func<IQueryable<LopHoc>, IOrderedQueryable<LopHoc>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "tenlop" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.TenLop))
                : (q => q.OrderBy(k => k.TenLop)),
            "phonghoc" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.PhongHoc))
                : (q => q.OrderBy(k => k.PhongHoc)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClassCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var khoaHoc = await _courseService.GetByIdAsync(request.KhoaHocId);
        
        if (khoaHoc == null) return BadRequest("Khóa học không tồn tại");   
        var lopHoc = new LopHoc
        {
            TenLop = request.TenLop,
            PhongHoc = request.PhongHoc,
            SiSoToiDa = request.SiSoToiDa,
            TrangThai = request.TrangThai,
            KhoaHoc = khoaHoc
        };

        var result = await _service.AddAsync(lopHoc);
        if (result == null) return BadRequest();

        return Ok(request);
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ClassUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingLopHoc = await _service.GetByIdAsync(id);
        if (existingLopHoc == null) return NotFound();

        var khoaHoc = await _courseService.GetByIdAsync(request.KhoaHocId ?? existingLopHoc.KhoaHoc.Id.ToString());

        existingLopHoc.TenLop = request.TenLop ?? existingLopHoc.TenLop;
        existingLopHoc.PhongHoc = request.PhongHoc ?? existingLopHoc.PhongHoc;
        existingLopHoc.SiSoToiDa = request.SiSoToiDa ?? existingLopHoc.SiSoToiDa;
        existingLopHoc.TrangThai = request.TrangThai ?? existingLopHoc.TrangThai;
        existingLopHoc.KhoaHoc = khoaHoc ?? existingLopHoc.KhoaHoc;

        await _service.UpdateAsync(existingLopHoc);

        return Ok();
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        await _service.DeleteAsync(entity);

        return Ok();
    }
}
