using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "admin,giaovu,giaovien,hocvien")]

public class LichHocController : BaseController<LichHoc>
{
    public LichHocController(IGenericService<LichHoc> service) : base(service)
    {

    }

    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetClassSchedule(string classId)
    {
        var schedule = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: int.MaxValue,
            Filter: s => s.LopHoc.Id.ToString() == classId,
            OrderBy: q => q.OrderBy(s => s.NgayHoc)
        );

        return Ok(schedule);
    }

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetTeacherSchedule(string teacherId)
    {
        var schedule = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: int.MaxValue,
            Filter: s => s.LopHoc.PhanCongs.Any(pc => pc.GiaoVien.Id.ToString() == teacherId),
            OrderBy: q => q.OrderBy(s => s.NgayHoc)
        );

        return Ok(schedule);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentSchedule(string studentId)
    {
        var schedule = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: int.MaxValue,
            Filter: s => s.LopHoc.DangKys.Any(hv => hv.Id.ToString() == studentId),
            OrderBy: q => q.OrderBy(s => s.NgayHoc)
        );

        return Ok(schedule);
    }
}