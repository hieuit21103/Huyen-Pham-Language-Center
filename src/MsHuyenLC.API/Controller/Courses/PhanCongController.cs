using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.DTOs.Teachers;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "admin,giaovu")]
public class PhanCongController : BaseController<PhanCong>
{
    private readonly IGenericService<GiaoVien> _teacherService;
    private readonly IGenericService<LopHoc> _classService;
    private readonly IGenericService<PhanCong> _assignmentService;

    public PhanCongController(
        IGenericService<PhanCong> service,
        IGenericService<GiaoVien> teacherService,
        IGenericService<LopHoc> classService,
        IGenericService<PhanCong> assignmentService) : base(service)
    {
        _teacherService = teacherService;
        _classService = classService;
        _assignmentService = assignmentService;
    }

    [HttpPost]
    public async Task<IActionResult> AssignTeacher([FromBody] AssignTeacherRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var giaoVien = await _teacherService.GetByIdAsync(request.GiaoVienId);

        if (giaoVien == null)
            return NotFound(new { message = "Không tìm thấy giáo viên" });

        var lopHoc = await _classService.GetByIdAsync(request.LopHocId);

        if (lopHoc == null)
            return NotFound(new { message = "Không tìm thấy lớp học" });

        if (lopHoc.PhanCongs.Any())
            return BadRequest(new { message = "Lớp học đã được phân công giáo viên" });

        var phanCong = new PhanCong
        {
            Id = Guid.NewGuid(),
            GiaoVien = giaoVien,
            LopHoc = lopHoc,
            NgayPhanCong = request.NgayPhanCong ?? DateTime.UtcNow
        };

        await _service.AddAsync(phanCong);

        var response = new TeacherAssignmentResponse
        {
            Id = phanCong.Id,
            GiaoVienId = giaoVien.Id,
            TenGiaoVien = giaoVien.HoTen,
            LopHocId = lopHoc.Id,
            TenLop = lopHoc.TenLop,
            TenKhoaHoc = lopHoc.KhoaHoc.TenKhoaHoc,
            NgayPhanCong = phanCong.NgayPhanCong
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeacherClasses(string id)
    {
        var giaoVien = await _teacherService.GetByIdAsync(id);

        if (giaoVien == null)
            return NotFound(new { message = "Không tìm thấy giáo viên" });

        var assignments = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: int.MaxValue,
            Filter: p => p.GiaoVien.Id.ToString() == id,
            OrderBy: q => q.OrderByDescending(p => p.NgayPhanCong)
            );

        return Ok(assignments);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveAssignment(string id)
    {
        var phanCong = await _service.GetByIdAsync(id);

        if (phanCong == null)
            return NotFound(new { message = "Không tìm thấy phân công" });

        await _service.DeleteAsync(phanCong);

        return Ok(new { message = "Đã hủy phân công giáo viên" });
    }
}