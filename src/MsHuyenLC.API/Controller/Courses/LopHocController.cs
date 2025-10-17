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
    protected readonly IGenericService<PhongHoc> _roomService;
    protected readonly IGenericService<DangKy> _registrationService;
    protected readonly IGenericService<PhanCong> _assignmentService;
    public LopHocController(
        IGenericService<LopHoc> service,
        IGenericService<KhoaHoc> courseService,
        IGenericService<PhongHoc> roomService,
        IGenericService<DangKy> registrationService,
        IGenericService<PhanCong> assignmentService) : base(service)
    {
        _courseService = courseService;
        _roomService = roomService;
        _registrationService = registrationService;
        _assignmentService = assignmentService;
    }

    protected override Func<IQueryable<LopHoc>, IOrderedQueryable<LopHoc>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "tenlop" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.TenLop))
                : (q => q.OrderBy(k => k.TenLop)),
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
        if (khoaHoc == null) return BadRequest("Khóa học không tồn tại");

        existingLopHoc.TenLop = request.TenLop ?? existingLopHoc.TenLop;
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

    [HttpGet("{id}/students")]
    [Authorize(Roles = "admin,giaovu,giaovien")]
    public async Task<IActionResult> GetStudentsInClass(string id)
    {
        var lopHoc = await _service.GetByIdAsync(id);
        if (lopHoc == null)
            return NotFound(new { message = "Không tìm thấy lớp học" });
        var assignment = (await _assignmentService.GetAllAsync(
            1,
            int.MaxValue,
            Filter: pc => pc.LopHoc.Id.ToString() == id
        )).ToList();
        if (!assignment.Any())
            return BadRequest(new { message = "Lớp học chưa được phân công giáo viên" });

        if (!assignment.First().GiaoVien.PhanCongs.Any(pc => pc.LopHoc.Id.ToString() == id && pc.GiaoVien.Id.ToString() == User.FindFirst("id")?.Value) &&
            !User.IsInRole("admin") && !User.IsInRole("giaovu"))
        {
            return Forbid();
        }

        var hocViens = await _registrationService.GetAllAsync(
            1,
            int.MaxValue,
            Filter: dk => dk.LopHoc.Id.ToString() == id,
            Includes: dk => dk.HocVien
        );

        return Ok(hocViens);
    }
}
