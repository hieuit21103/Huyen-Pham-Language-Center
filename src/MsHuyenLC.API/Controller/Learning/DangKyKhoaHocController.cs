using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.DangKyKhoaHoc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.Learning;

namespace MsHuyenLC.API.Controller.Learning;

[Route("api/[controller]")]
[ApiController]
public class DangKyController : BaseController
{
    private readonly IRegistrationService _service;
    public DangKyController(
        IRegistrationService service,
        ISystemLoggerService logService)
        : base(logService)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var entities = await _service.GetAllAsync();

        var response = entities.Select(dk => new DangKyHocVienResponse
        {
            Id = dk.Id,
            HocVien = dk.HocVien,
            KhoaHoc = dk.KhoaHoc,
            LopHoc = dk.LopHoc,
            NgayDangKy = dk.NgayDangKy,
            TrangThai = dk.TrangThai
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đăng ký thành công",
            data = response
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetDangKyById(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy đăng ký"
            });
        }

        var response = new DangKyHocVienResponse
        {
            Id = entity.Id,
            KhoaHoc = entity.KhoaHoc,
            LopHoc = entity.LopHoc,
            NgayDangKy = entity.NgayDangKy,
            TrangThai = entity.TrangThai
        };

        return Ok(new
        {
            success = true,
            message = "Lấy đăng ký thành công",
            data = response
        });
    }

    [HttpGet("student/{hocVienId}")]
    [Authorize]
    public async Task<IActionResult> GetDangKyByHocVienId(string hocVienId)
    {
        var entities = await _service.GetByStudentIdAsync(hocVienId);

        var response = entities.Select(dk => new DangKyHocVienResponse
        {
            Id = dk.Id,
            KhoaHoc = dk.KhoaHoc,
            LopHoc = dk.LopHoc,
            NgayDangKy = dk.NgayDangKy,
            TrangThai = dk.TrangThai
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Lấy đăng ký của học viên thành công",
            data = response
        });
    }

    [HttpPost("student")]
    [Authorize]
    public async Task<IActionResult> RegisterStudent([FromBody] DangKyKhoaHocRequest request)
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
                message = "Đăng ký thất bại"
            });

        await LogCreateAsync(result);

        var response = new DangKyKhoaHocResponse
        {
            Id = result.Id,
            HocVienId = result.HocVienId,
            LopHocId = result.LopHocId,
            NgayDangKy = result.NgayDangKy
        };

        return Ok(new
        {
            success = true,
            message = "Đăng ký thành công",
            data = response
        });
    }

    [HttpPost]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> CreateDangKy([FromBody] DangKyKhoaHocCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ",
                errors = ModelState
            });

        var result = await _service.CreateFullAsync(request);
        if (result == null)
            return BadRequest(new
            {
                success = false,
                message = "Tạo đăng ký thất bại"
            });

        await LogCreateAsync(result);

        var response = new DangKyKhoaHocResponse
        {
            Id = result.Id,
            HocVienId = result.HocVienId,
            LopHocId = result.LopHocId,
            NgayDangKy = result.NgayDangKy
        };

        return Ok(new
        {
            success = true,
            message = "Tạo đăng ký thành công",
            data = response
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> UpdateDangKy(string id, [FromBody] DangKyKhoaHocUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ",
                errors = ModelState
            });

        var existingDangKy = await _service.GetByIdAsync(id);
        if (existingDangKy == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy đăng ký"
            });

        var oldData = new DangKyKhoaHoc
        {
            Id = existingDangKy.Id,
            LopHocId = existingDangKy.LopHocId,
            NgayXepLop = existingDangKy.NgayXepLop,
            TrangThai = existingDangKy.TrangThai,
            NgayDangKy = existingDangKy.NgayDangKy,
            KhoaHocId = existingDangKy.KhoaHocId,
            HocVienId = existingDangKy.HocVienId
        };

        await _service.UpdateAsync(id, request);

        await LogUpdateAsync(oldData, existingDangKy);

        var response = new DangKyKhoaHocResponse
        {
            Id = existingDangKy.Id,
            HocVienId = existingDangKy.HocVienId,
            LopHocId = existingDangKy.LopHocId,
            NgayDangKy = existingDangKy.NgayDangKy,
            NgayXepLop = existingDangKy.NgayXepLop,
            KhoaHocId = existingDangKy.KhoaHocId,
        };

        return Ok(new
        {
            success = true,
            message = "Cập nhật đăng ký thành công",
            data = response
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> DeleteDangKy(string id)
    {
        var existingDangKy = await _service.GetByIdAsync(id);
        if (existingDangKy == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy đăng ký"
            });
        await _service.DeleteAsync(id);
        await LogDeleteAsync(existingDangKy);
        return Ok(new
        {
            success = true,
            message = "Xóa đăng ký thành công"
        });
    }
}


