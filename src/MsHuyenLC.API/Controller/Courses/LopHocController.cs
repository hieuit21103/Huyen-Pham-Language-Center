using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;
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
        ISystemLoggerService logService,
        IGenericService<KhoaHoc> courseService,
        IGenericService<PhongHoc> roomService,
        IGenericService<DangKy> registrationService,
        IGenericService<PhanCong> assignmentService) : base(service, logService)
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
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var khoaHoc = await _courseService.GetByIdAsync(request.KhoaHocId.ToString());
        if (khoaHoc == null) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Khóa học không tồn tại" 
            });

        var lopHoc = new LopHoc
        {
            TenLop = request.TenLop,
            SiSoToiDa = request.SiSoToiDa,
            KhoaHocId = request.KhoaHocId
        };

        var result = await _service.AddAsync(lopHoc);
        if (result == null) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Tạo lớp học thất bại" 
            });

        await LogCreateAsync(result);

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

        return Ok(new
        {
            success = true,
            message = "Tạo lớp học thành công",
            data = response
        });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] LopHocUpdateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var existingLopHoc = await _service.GetByIdAsync(id);
        if (existingLopHoc == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lớp học" 
            });

        var oldData = new LopHoc
        {
            Id = existingLopHoc.Id,
            TenLop = existingLopHoc.TenLop,
            SiSoToiDa = existingLopHoc.SiSoToiDa,
            SiSoHienTai = existingLopHoc.SiSoHienTai,
            TrangThai = existingLopHoc.TrangThai,
            KhoaHocId = existingLopHoc.KhoaHocId
        };

        existingLopHoc.TenLop = request.TenLop;
        existingLopHoc.SiSoToiDa = request.SiSoToiDa;
        existingLopHoc.TrangThai = request.TrangThai;

        await _service.UpdateAsync(existingLopHoc);
        await LogUpdateAsync(oldData, existingLopHoc);

        return Ok(new
        {
            success = true,
            message = "Cập nhật lớp học thành công",
            data = existingLopHoc
        });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) 
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lớp học" 
            });
        
        await _service.DeleteAsync(entity);
        await LogDeleteAsync(entity);

        return Ok(new
        {
            success = true,
            message = "Xóa lớp học thành công"
        });
    }

    [HttpGet("{id}/students")]
    [Authorize(Roles = "admin,giaovu,giaovien")]
    public async Task<IActionResult> GetStudentsInClass(string id)
    {
        var lopHoc = await _service.GetByIdAsync(id);
        if (lopHoc == null)
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy lớp học" 
            });
        
        var assignment = (await _assignmentService.GetAllAsync(
            1,
            int.MaxValue,
            Filter: pc => pc.LopHocId.ToString() == id
        )).ToList();
        
        if (!assignment.Any())
            return BadRequest(new 
            { 
                success = false, 
                message = "Lớp học chưa được phân công giáo viên" 
            });

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
        var DanhSachHocVien = hocViens
            .Select(dk => new HocVienLopResponse
            {
                Id = dk.HocVien.Id.ToString(),
                HoTen = dk.HocVien.HoTen,
                Email = dk.HocVien.TaiKhoan.Email ?? "",
                Sdt = dk.HocVien.TaiKhoan.Sdt ?? "",
                GioiTinh = dk.HocVien.GioiTinh ?? 0
            })
            .ToList();
        var response = new DanhSachLopResponse
        {
            Id = lopHoc.Id.ToString(),
            TenLop = lopHoc.TenLop,
            DanhSachHocVien = DanhSachHocVien
        };

        return Ok(new {
            success = true,
            message = "Lấy danh sách học viên thành công",
            data = response
        });
    }
}
