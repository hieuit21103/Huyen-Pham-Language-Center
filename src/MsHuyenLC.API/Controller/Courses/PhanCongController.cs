using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.DTOs.Courses.PhanCong;
using MsHuyenLC.Application.Services.Courses;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "admin,giaovu")]
public class PhanCongController : BaseController<PhanCong>
{
    private readonly IGenericService<GiaoVien> _teacherService;
    private readonly IGenericService<LopHoc> _classService;

    public PhanCongController(
        IGenericService<PhanCong> service,
        ISystemLoggerService logService,
        IGenericService<GiaoVien> teacherService,
        IGenericService<LopHoc> classService) : base(service, logService)
    {
        _teacherService = teacherService;
        _classService = classService;
    }

    [HttpPost]
    public async Task<IActionResult> AssignTeacher([FromBody] PhanCongRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var giaoVien = await _teacherService.GetByIdAsync(request.GiaoVienId.ToString());

        if (giaoVien == null)
            return NotFound(new { message = "Không tìm thấy giáo viên" });

        var lopHoc = await _classService.GetByIdAsync(request.LopHocId.ToString());

        if (lopHoc == null)
            return NotFound(new { message = "Không tìm thấy lớp học" });

        var existingAssignment = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: int.MaxValue,
            Filter: p => p.LopHocId == request.LopHocId
        );
        
        if (existingAssignment.Any())
            return BadRequest(new { message = "Lớp học đã được phân công giáo viên" });

        var phanCong = new PhanCong
        {
            GiaoVienId = request.GiaoVienId,
            LopHocId = request.LopHocId
        };

        var result = await _service.AddAsync(phanCong);
        if (result == null)
            return BadRequest(new { message = "Phân công thất bại" });
        
        await LogCreateAsync(result);
        
        var response = new PhanCongResponse
        {
            Id = result.Id,
            GiaoVienId = result.GiaoVienId,
            TenGiaoVien = giaoVien.HoTen,
            LopHocId = result.LopHocId,
            TenLop = lopHoc.TenLop,
            NgayPhanCong = result.NgayPhanCong
        };
        
        return Ok(response);
    }

    [HttpGet("giaovien/{id}")]
    public async Task<IActionResult> GetTeacherClasses(string id)
    {
        var giaoVien = await _teacherService.GetByIdAsync(id);

        if (giaoVien == null)
            return NotFound(new { message = "Không tìm thấy giáo viên" });

        var assignments = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: int.MaxValue,
            Filter: p => p.GiaoVienId.ToString() == id,
            OrderBy: q => q.OrderByDescending(p => p.NgayPhanCong)
        );

        var responses = new List<PhanCongResponse>();
        foreach (var assignment in assignments)
        {
            var lopHoc = await _classService.GetByIdAsync(assignment.LopHocId.ToString());
            responses.Add(new PhanCongResponse
            {
                Id = assignment.Id,
                GiaoVienId = assignment.GiaoVienId,
                TenGiaoVien = giaoVien.HoTen,
                LopHocId = assignment.LopHocId,
                TenLop = lopHoc?.TenLop ?? "",
                NgayPhanCong = assignment.NgayPhanCong
            });
        }

        return Ok(responses);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var phanCong = await _service.GetByIdAsync(id);

        if (phanCong == null)
            return NotFound(new { message = "Không tìm thấy phân công" });

        await _service.DeleteAsync(phanCong);
        await LogDeleteAsync(phanCong);

        return Ok(new { message = "Đã hủy phân công giáo viên" });
    }
}
