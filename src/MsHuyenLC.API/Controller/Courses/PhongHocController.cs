using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Classes;

namespace MsHuyenLC.API.Controller.Courses;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class PhongHocController : BaseController<PhongHoc>
{
    public PhongHocController(IGenericService<PhongHoc> service) : base(service)
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
    public async Task<IActionResult> Create([FromBody] RoomCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var phongHoc = new PhongHoc
        {
            TenPhong = request.TenPhong,
            SoGhe = request.SoGhe
        };

        var result = await _service.AddAsync(phongHoc);
        if (result == null) return BadRequest();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] RoomCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingRoom = await _service.GetByIdAsync(id);
        if (existingRoom == null) return NotFound();

        existingRoom.TenPhong = request.TenPhong ?? existingRoom.TenPhong;
        existingRoom.SoGhe = request.SoGhe != 0 ? request.SoGhe : existingRoom.SoGhe;

        await _service.UpdateAsync(existingRoom);

        return Ok(existingRoom);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingRoom = await _service.GetByIdAsync(id);
        if (existingRoom == null) return NotFound();

        await _service.DeleteAsync(existingRoom);

        return Ok();
    }
}