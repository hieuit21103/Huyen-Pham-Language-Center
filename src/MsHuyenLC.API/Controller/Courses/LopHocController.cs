using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.LopHoc;
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
    public async Task<IActionResult> Create([FromBody] LopHocRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var khoaHoc = await _courseService.GetByIdAsync(request.KhoaHocId.ToString());
        if (khoaHoc == null) return BadRequest("Khóa học không tồn tại");

        var lopHoc = new LopHoc
        {
            TenLop = request.TenLop,
            SiSoToiDa = request.SiSoToiDa,
            KhoaHocId = request.KhoaHocId
        };

        var result = await _service.AddAsync(lopHoc);
        if (result == null) return BadRequest();

        var response = new LopHocResponse
        {
            Id = result.Id,
            TenLop = result.TenLop,
            SiSoToiDa = result.SiSoToiDa,
            SiSoHienTai = result.SiSoHienTai,
            TrangThai = result.TrangThai,
            KhoaHocId = result.KhoaHocId,
            TenKhoaHoc = khoaHoc.TenKhoaHoc
        };

        return Ok(response);
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] LopHocUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingLopHoc = await _service.GetByIdAsync(id);
        if (existingLopHoc == null) return NotFound();

        existingLopHoc.TenLop = request.TenLop;
        existingLopHoc.SiSoToiDa = request.SiSoToiDa;
        existingLopHoc.TrangThai = request.TrangThai;

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
            Filter: pc => pc.LopHocId.ToString() == id
        )).ToList();
        
        if (!assignment.Any())
            return BadRequest(new { message = "Lớp học chưa được phân công giáo viên" });

        var currentUserId = User.FindFirst("id")?.Value;
        if (!User.IsInRole("admin") && !User.IsInRole("giaovu") && 
            assignment.First().GiaoVienId.ToString() != currentUserId)
        {
            return Forbid();
        }

        var hocViens = await _registrationService.GetAllAsync(
            1,
            int.MaxValue,
            Filter: dk => dk.LopHocId.ToString() == id,
            Includes: dk => dk.HocVien
        );

        return Ok(hocViens);
    }
}
