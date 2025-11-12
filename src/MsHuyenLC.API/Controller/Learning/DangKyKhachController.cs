using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.DangKyKhach;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.Auth;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.Course;

namespace MsHuyenLC.API.Controller.Learning;

[Route("api/[controller]")]
[ApiController]
public class DangKyKhachController : BaseController
{
    private readonly IGuestRegistrationService _service;
    private readonly ICourseService _courseService;

    public DangKyKhachController(
        ISystemLoggerService logService,
        IGuestRegistrationService service,
        ICourseService courseService
    ) : base(logService)
    {
        _service = service;
        _courseService = courseService;
    }

    [HttpGet]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetAll()
    {
        var entities = await _service.GetAllAsync();

        var response = entities.Select(dk => new DangKyKhachResponse
        {
            Id = dk.Id,
            HoTen = dk.HoTen,
            GioiTinh = dk.GioiTinh,
            Email = dk.Email,
            SoDienThoai = dk.SoDienThoai,
            NoiDung = dk.NoiDung,
            NgayDangKy = dk.NgayDangKy,
            TrangThai = dk.TrangThai,
            KetQua = dk.KetQua,
            KhoaHocId = dk.KhoaHocId
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đăng ký khách thành công",
            data = response
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetById(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy đăng ký khách"
            });
        }

        var response = new DangKyKhachResponse
        {
            Id = entity.Id,
            HoTen = entity.HoTen,
            GioiTinh = entity.GioiTinh,
            Email = entity.Email,
            SoDienThoai = entity.SoDienThoai,
            NoiDung = entity.NoiDung,
            NgayDangKy = entity.NgayDangKy,
            TrangThai = entity.TrangThai,
            KetQua = entity.KetQua,
            NgayXuLy = entity.NgayXuLy,
            KhoaHocId = entity.KhoaHocId
        };

        return Ok(new
        {
            success = true,
            message = "Lấy đăng ký khách thành công",
            data = response
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> GuestRegister([FromBody] DangKyKhachRequest request)
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

        var response = new DangKyKhachResponse
        {
            Id = result.Id,
            HoTen = result.HoTen,
            GioiTinh = result.GioiTinh,
            Email = result.Email,
            SoDienThoai = result.SoDienThoai,
            NoiDung = result.NoiDung,
            NgayDangKy = result.NgayDangKy,
            TrangThai = result.TrangThai,
            KetQua = result.KetQua,
            KhoaHocId = result.KhoaHocId,
            TenKhoaHoc = result.KhoaHoc.TenKhoaHoc
        };

        return Ok(new
        {
            success = true,
            message = "Đăng ký thành công. Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất",
            data = response
        });
    }


    [HttpPost("create")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> CreateByAdmin([FromBody] DangKyKhachCreateRequest request)
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

        var response = new DangKyKhachResponse
        {
            Id = result.Id,
            HoTen = result.HoTen,
            Email = result.Email,
            SoDienThoai = result.SoDienThoai,
            NoiDung = result.NoiDung,
            NgayDangKy = result.NgayDangKy,
            TrangThai = result.TrangThai,
            KetQua = result.KetQua,
            GioiTinh = result.GioiTinh,
            KhoaHocId = result.KhoaHocId,
            TenKhoaHoc = result.KhoaHoc.TenKhoaHoc,
            TaiKhoanXuLyId = result.TaiKhoanXuLyId
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
    public async Task<IActionResult> Update(string id, [FromBody] DangKyKhachUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ",
                errors = ModelState
            });

        var existing = await _service.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy đăng ký"
            });

        var oldData = new DangKyKhach
        {
            Id = existing.Id,
            HoTen = existing.HoTen,
            Email = existing.Email,
            SoDienThoai = existing.SoDienThoai,
            NoiDung = existing.NoiDung,
            KhoaHocId = existing.KhoaHocId,
            TrangThai = existing.TrangThai,
            KetQua = existing.KetQua,
            GioiTinh = existing.GioiTinh,
            NgayDangKy = existing.NgayDangKy,
            NgayXuLy = existing.NgayXuLy,
            TaiKhoanXuLyId = existing.TaiKhoanXuLyId
        };

        var previousTrangThai = existing.TrangThai;
        // Service sẽ xử lý tất cả logic (bao gồm tạo tài khoản, học viên, thanh toán khi duyệt)
        var updated = await _service.UpdateAsync(id, request);
        if (updated == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Cập nhật đăng ký thất bại"
            });
        }

        await LogUpdateAsync(oldData, updated);

        var message = updated.TrangThai == TrangThaiDangKy.daduyet && previousTrangThai != TrangThaiDangKy.daduyet
            ? "Đã duyệt và tạo tài khoản thành công. Thông tin đăng nhập: Email = " + updated.Email + ", Mật khẩu = Số điện thoại"
            : "Cập nhật đăng ký thành công";

        return Ok(new
        {
            success = true,
            message
        });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy đăng ký"
            });

        await _service.DeleteAsync(id);
        await LogDeleteAsync(existing);

        return Ok(new
        {
            success = true,
            message = "Xóa đăng ký thành công"
        });
    }


    [HttpGet("statistics")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> GetStatistics()
    {
        var allRegistrations = await _service.GetAllAsync();

        var stats = new
        {
            TongSoDangKy = allRegistrations.Count(),
            ChoDuyet = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.choduyet),
            DaDuyet = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.daduyet),
            DaXepLop = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.daxeplop),
            Huy = allRegistrations.Count(d => d.TrangThai == TrangThaiDangKy.huy),
            ChuaXuLy = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.chuaxuly),
            DaXuLy = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.daxuly),
            DaTuChoi = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.datuchoi),
            DaHuy = allRegistrations.Count(d => d.KetQua == KetQuaDangKy.dahuy),
            DangKyTrongThang = allRegistrations.Count(d => d.NgayDangKy.Month == DateTime.UtcNow.Month &&
                                                           d.NgayDangKy.Year == DateTime.UtcNow.Year)
        };

        return Ok(new
        {
            success = true,
            message = "Lấy thống kê thành công",
            data = stats
        });
    }
}


