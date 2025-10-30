using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.DTOs.Learning.PhanHoi;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu,hocvien")]

public class PhanHoiController : BaseController<PhanHoi>
{
    public PhanHoiController(IGenericService<PhanHoi> service) : base(service)
    {
    }

    protected override Func<IQueryable<PhanHoi>, IOrderedQueryable<PhanHoi>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy.ToLower() switch
        {
            "ngaytao" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(e => e.NgayTao))
                : (q => q.OrderBy(e => e.NgayTao)),
            _ => null
        };
    }

    [HttpGet]
    public override async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "asc"
    )
    {
        var orderBy = BuildOrderBy(sortBy, sortOrder);
        var entities = await _service.GetAllAsync(
            pageNumber,
            pageSize,
            null,
            orderBy
        );
        var response = entities.Select(e => new PhanHoiResponse
        {
            Id = e.Id,
            HocVienId = e.HocVienId,
            TenHocVien = e.HocVien?.HoTen,
            LoaiPhanHoi = e.LoaiPhanHoi,
            TieuDe = e.TieuDe,
            NoiDung = e.NoiDung,
            NgayTao = e.NgayTao
        }).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetById(string id)
    {
        var request = await _service.GetByIdAsync(id);
        if (request == null) return NotFound();

        var response = new PhanHoiResponse
        {
            Id = request.Id,
            HocVienId = request.HocVienId,
            TenHocVien = request.HocVien?.HoTen,
            LoaiPhanHoi = request.LoaiPhanHoi,
            TieuDe = request.TieuDe,
            NoiDung = request.NoiDung,
            NgayTao = request.NgayTao
        };
        return Ok(response);
    }                   

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PhanHoiRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var phanHoi = new PhanHoi
        {
            HocVienId = request.HocVienId,
            LoaiPhanHoi = request.LoaiPhanHoi,
            TieuDe = request.TieuDe,
            NoiDung = request.NoiDung,
        };
        var createdPhanHoi = await _service.AddAsync(phanHoi);
        if (createdPhanHoi == null)
        {
            return BadRequest(new { message = "Tạo phản hồi thất bại." });
        }
        return Ok(new
        {
            success = true,
            message = "Tạo phản hồi thành công.",
            data = createdPhanHoi
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] PhanHoiRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingPhanHoi = await _service.GetByIdAsync(id);
        if (existingPhanHoi == null)
        {
            return NotFound(new { message = "Phản hồi không tồn tại." });
        }
        existingPhanHoi.HocVienId = request.HocVienId;
        existingPhanHoi.LoaiPhanHoi = request.LoaiPhanHoi;
        existingPhanHoi.TieuDe = request.TieuDe;
        existingPhanHoi.NoiDung = request.NoiDung;

        await _service.UpdateAsync(existingPhanHoi);
        return Ok(new
        {
            success = true,
            message = "Cập nhật phản hồi thành công.",
            data = existingPhanHoi
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingPhanHoi = await _service.GetByIdAsync(id);
        if (existingPhanHoi == null)
        {
            return NotFound(new { message = "Phản hồi không tồn tại." });
        }

        await _service.DeleteAsync(existingPhanHoi);
        return Ok(new { success = true, message = "Xóa phản hồi thành công." });
    }
}