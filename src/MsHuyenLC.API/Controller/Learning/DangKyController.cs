using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.DangKy;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Learning;

[Route("api/[controller]")]
[ApiController]
public class DangKyController : BaseController<DangKy>
{
    private readonly IGenericService<LopHoc> _lopHocService;
    private readonly IGenericService<ThanhToan> _thanhToanService;
    public DangKyController(
        IGenericService<DangKy> service,
        ISystemLoggerService logService,
        IGenericService<LopHoc> lopHocService,
        IGenericService<ThanhToan> thanhToanService)
        : base(service, logService)
    {
        _lopHocService = lopHocService;
        _thanhToanService = thanhToanService;
    }

    protected override Func<IQueryable<DangKy>, IOrderedQueryable<DangKy>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "ngaydangky" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.NgayDangKy))
                : (q => q.OrderBy(k => k.NgayDangKy)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [HttpGet("student/{hocVienId}")]
    [Authorize]
    public async Task<IActionResult> GetDangKyByHocVienId(string hocVienId)
    {
        var entities = await _service.GetAllAsync(
            1,
            int.MaxValue,
            Filter: dk => dk.HocVienId.ToString() == hocVienId
        );

        var response = entities.Select(dk => new DangKyHocVienResponse
        {
            Id = dk.Id,
            KhoaHoc = dk.KhoaHoc,
            LopHoc = dk.LopHoc,
            NgayDangKy = dk.NgayDangKy,
            TrangThai = dk.TrangThai
        }).ToList();

        return Ok(new{
            success = true,
            message = "Lấy đăng ký của học viên thành công",
            data = response
        });
    }

    [HttpPost("student")]
    [Authorize]
    public async Task<IActionResult> RegisterStudent([FromBody] DangKyRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var dangKy = new DangKy
        {
            KhoaHocId = request.KhoaHocId,
            HocVienId = request.HocVienId,
            TrangThai = 0,
        };

        var result = await _service.AddAsync(dangKy);
        if (result == null) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Đăng ký thất bại" 
            });

        await LogCreateAsync(result);

        var response = new DangKyResponse
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
    public async Task<IActionResult> CreateDangKy([FromBody] DangKyCreateRequest request)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Dữ liệu không hợp lệ",
                errors = ModelState 
            });

        var dangKy = new DangKy
        {
            NgayDangKy = request.NgayDangKy ?? DateOnly.FromDateTime(DateTime.UtcNow),
            LopHocId = request.LopHocId ?? null,
            NgayXepLop = request.NgayXepLop ?? null,
            KhoaHocId = request.KhoaHocId,
            HocVienId = request.HocVienId,
            TrangThai = 0,
        };

        var result = await _service.AddAsync(dangKy);
        if (result == null) 
            return BadRequest(new 
            { 
                success = false, 
                message = "Tạo đăng ký thất bại" 
            });

        await LogCreateAsync(result);

        var response = new DangKyResponse
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
    public async Task<IActionResult> UpdateDangKy(string id, [FromBody] DangKyUpdateRequest request)
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


        if (existingDangKy.LopHocId == null)
        {
            if (request.LopHocId != null)
            {
                var existingLopHoc = await _lopHocService.GetByIdAsync(request.LopHocId.Value.ToString());
                if (existingLopHoc != null)
                {
                    existingLopHoc.CapNhatSiSo(1);
                    await _lopHocService.UpdateAsync(existingLopHoc);
                }
            }
        }
        else
        {
            var oldLopHoc = await _lopHocService.GetByIdAsync(existingDangKy.LopHocId.Value.ToString());
            if (oldLopHoc != null && request.LopHocId != null && oldLopHoc.Id != request.LopHocId.Value)
            {
                oldLopHoc.CapNhatSiSo(-1);
                await _lopHocService.UpdateAsync(oldLopHoc);

                var newLopHoc = await _lopHocService.GetByIdAsync(request.LopHocId.Value.ToString());
                if (newLopHoc != null)
                {
                    newLopHoc.CapNhatSiSo(1);
                    await _lopHocService.UpdateAsync(newLopHoc);
                }
            }
        }

        var oldData = new DangKy
        {
            Id = existingDangKy.Id,
            LopHocId = existingDangKy.LopHocId,
            NgayXepLop = existingDangKy.NgayXepLop,
            TrangThai = existingDangKy.TrangThai,
            NgayDangKy = existingDangKy.NgayDangKy,
            KhoaHocId = existingDangKy.KhoaHocId,
            HocVienId = existingDangKy.HocVienId
        };

        existingDangKy.LopHocId = request.LopHocId ?? existingDangKy.LopHocId;
        existingDangKy.NgayXepLop = request.NgayXepLop ?? existingDangKy.NgayXepLop;
        existingDangKy.TrangThai = request.TrangThai ?? existingDangKy.TrangThai;
        existingDangKy.NgayDangKy = request.NgayDangKy ?? existingDangKy.NgayDangKy;
        existingDangKy.KhoaHocId = request.KhoaHocId ?? existingDangKy.KhoaHocId;
        existingDangKy.HocVienId = request.HocVienId ?? existingDangKy.HocVienId;

        await _service.UpdateAsync(existingDangKy);

        if( existingDangKy.TrangThai == TrangThaiDangKy.daduyet)
        {
            var existingThanhToan = await _thanhToanService.GetAllAsync(
                1,
                1,
                Filter: tt => tt.DangKyId == existingDangKy.Id
            );

            if (!existingThanhToan.Any())
            {
                var thanhToan = new ThanhToan
                {
                    DangKyId = existingDangKy.Id,
                    SoTien = existingDangKy.KhoaHoc.HocPhi,
                };

                var result = await _thanhToanService.AddAsync(thanhToan);
                if( result == null)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Tạo thanh toán thất bại" 
                    });
                }
            }
        }

        await LogUpdateAsync(oldData, existingDangKy);

        var response = new DangKyResponse
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

        var existingLopHoc = await _lopHocService.GetByIdAsync(existingDangKy.LopHocId.ToString());
        if (existingLopHoc != null)
        {
            existingLopHoc.CapNhatSiSo(-1);
            await _lopHocService.UpdateAsync(existingLopHoc);
        }
        await _service.DeleteAsync(existingDangKy);

        await LogDeleteAsync(existingDangKy);

        return Ok(new
        {
            success = true,
            message = "Xóa đăng ký thành công"
        });
    }
}
