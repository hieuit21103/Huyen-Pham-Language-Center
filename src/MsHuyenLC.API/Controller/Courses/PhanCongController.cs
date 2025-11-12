using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Courses.PhanCong;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.Course;
using MsHuyenLC.Application.Interfaces.Services.User;
using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.API.Controller.Courses;

[Route("api/[controller]")]
[ApiController]
public class PhanCongController : BaseController<PhanCong>
{
    private readonly IAssignmentService _service;
    private readonly ITeacherService _teacherService;
    private readonly IClassService _classService;
    private readonly IUnitOfWork _unitOfWork;

    public PhanCongController(
        IAssignmentService service,
        ISystemLoggerService logService,
        ITeacherService teacherService,
        IClassService classService,
        IUnitOfWork unitOfWork) : base(logService)
    {
        _service = service;
        _teacherService = teacherService;
        _classService = classService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var assignments = await _service.GetAllAsync();

        var responses = new List<PhanCongResponse>();
        foreach (var assignment in assignments)
        {
            var giaoVien = await _teacherService.GetByIdAsync(assignment.GiaoVienId.ToString());
            var lopHoc = await _classService.GetByIdAsync(assignment.LopHocId.ToString());
            
            responses.Add(new PhanCongResponse
            {
                Id = assignment.Id,
                GiaoVienId = assignment.GiaoVienId,
                TenGiaoVien = giaoVien?.HoTen ?? "",
                LopHocId = assignment.LopHocId,
                TenLop = lopHoc?.TenLop ?? "",
                NgayPhanCong = assignment.NgayPhanCong
            });
        }

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách phân công thành công",
            data = responses
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

        var giaoVien = await _teacherService.GetByIdAsync(assignment.GiaoVienId.ToString());
        var lopHoc = await _classService.GetByIdAsync(assignment.LopHocId.ToString());

        var response = new PhanCongResponse
        {
            Id = assignment.Id,
            GiaoVienId = assignment.GiaoVienId,
            TenGiaoVien = giaoVien?.HoTen ?? "",
            LopHocId = assignment.LopHocId,
            TenLop = lopHoc?.TenLop ?? "",
            NgayPhanCong = assignment.NgayPhanCong
        };

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin phân công thành công",
            data = response
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

        var giaoVien = await _teacherService.GetByIdAsync(request.GiaoVienId.ToString());

        if (giaoVien == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo viên" 
            });

        var lopHoc = await _classService.GetByIdAsync(request.LopHocId.ToString());

        if (lopHoc == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lớp học" 
            });

        var existingAssignment = await _service.GetByClassIdAsync(request.LopHocId.ToString());
        
        if (existingAssignment != null)
            return BadRequest(new 
            { 
                success = false, 
                message = "Lớp học đã được phân công giáo viên" 
            });

        var result = await _service.CreateAsync(request);
        if (result == null)
            return BadRequest(new 
            { 
                success = false, 
                message = "Phân công thất bại" 
            });
        
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
        
        return Ok(new
        {
            success = true,
            message = "Phân công giáo viên thành công",
            data = response
        });
    }

    [HttpGet("giaovien/{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetTeacherClasses(string id)
    {
        var giaoVien = await _teacherService.GetByIdAsync(id);

        if (giaoVien == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy giáo viên" 
            });

        var assignments = await _service.GetAllByTeacherIdAsync(id);

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

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách lớp học của giáo viên thành công",
            data = responses
        });
    }

    [HttpGet("lophoc/{id}")]
    [Authorize(Roles = "admin,giaovu,giaovien,hocvien")]
    public async Task<IActionResult> GetClassTeacher(string id)
    {
        var lopHoc = await _classService.GetByIdAsync(id);

        if (lopHoc == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lớp học" 
            });

        var phanCong = await _service.GetByClassIdAsync(id);

        if (phanCong == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Lớp học chưa được phân công giáo viên" 
            });

        var giaoVien = await _teacherService.GetByIdAsync(phanCong.GiaoVienId.ToString());

        var response = new PhanCongResponse
        {
            Id = phanCong.Id,
            GiaoVienId = phanCong.GiaoVienId,
            TenGiaoVien = giaoVien?.HoTen ?? "",
            LopHocId = phanCong.LopHocId,
            TenLop = lopHoc.TenLop,
            NgayPhanCong = phanCong.NgayPhanCong
        };

        return Ok(new 
        {
            success = true,
            message = "Lấy thông tin giáo viên của lớp học thành công",
            data = response
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


