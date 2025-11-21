using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.PhanCong;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.Course;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
public class PhanCongController : BaseController
{
    private readonly IAssignmentService _service;

    public PhanCongController(
        IAssignmentService service,
        ISystemLoggerService logService) : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var assignments = await _service.GetAllAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách phân công thành công",
            data = assignments
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetById(string id)
    {
        var assignment = await _service.GetByIdAsync(id);
        if (assignment == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy phân công"
            });

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin phân công thành công",
            data = assignment
        });
    }

    [HttpPost]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> AssignTeacher([FromBody] PhanCongRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var result = await _service.CreateAsync(request);
        if (result == null)
            return BadRequest(new
            {
                success = false,
                message = "Phân công giáo viên thất bại"
            });
        await LogCreateAsync(result);
        
        return Ok(new
        {
            success = true,
            message = "Phân công giáo viên thành công",
            data = result
        });
    }

    [HttpGet("giaovien/{id}")]
    [Authorize(Roles = "admin,giaovu,giaovien")]
    public async Task<IActionResult> GetTeacherClasses(string id)
    {
        var phanCongs = await _service.GetByTeacherIdAsync(id);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách lớp học của giáo viên thành công",
            data = phanCongs
        });
    }

    [HttpGet("lophoc/{id}")]
    [Authorize(Roles = "admin,giaovu,giaovien,hocvien")]
    public async Task<IActionResult> GetClassTeacher(string id)
    {
        var phanCong = await _service.GetByClassIdAsync(id);

        if (phanCong == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Lớp học chưa được phân công giáo viên" 
            });

        return Ok(new 
        {
            success = true,
            message = "Lấy thông tin giáo viên của lớp học thành công",
            data = phanCong
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Delete(string id)
    {
        var phanCong = await _service.GetByIdAsync(id);
        if (phanCong == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy phân công" 
            });

        await LogDeleteAsync(phanCong);

        var result = await _service.DeleteAsync(id);
        if (!result)
            return BadRequest(new
            {
                success = false,
                message = "Xóa phân công thất bại"
            });

        return Ok(new 
        { 
            success = true, 
            message = "Xóa phân công thành công" 
        });
    }
}


