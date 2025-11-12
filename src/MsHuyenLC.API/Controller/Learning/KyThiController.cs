using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class KyThiController : BaseController
{
    private readonly IExamSessionService _service;
    public KyThiController(ISystemLoggerService loggerService, IExamSessionService service) : base(loggerService)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<KyThi>>> GetAll()
    {
        var kyThis = await _service.GetAllAsync();
        return Ok(new
        {
            success = true,
            data = kyThis
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<KyThi>> GetById(string id)
    {
        var kyThi = await _service.GetByIdAsync(id);
        if (kyThi == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy kỳ thi"
            });
        }

        return Ok(new
        {
            success = true,
            data = kyThi
        });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] KyThiRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ",
                    errors = ModelState
                });
            }

            var createdKyThi = await _service.CreateAsync(request);

            return Ok(new
            {
                success = true,
                message = "Tạo kỳ thi thành công",
                data = new
                {
                    id = createdKyThi.Id,
                    tenKyThi = createdKyThi.TenKyThi,
                    ngayThi = createdKyThi.NgayThi,
                    thoiLuong = createdKyThi.ThoiLuong,
                    lopHocId = createdKyThi.LopHocId,
                    trangThai = createdKyThi.TrangThai
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi tạo kỳ thi",
                error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] KyThiUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ",
                    errors = ModelState
                });
            }

            var existingKyThi = await _service.GetByIdAsync(id);
            if (existingKyThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kỳ thi"
                });
            }
            var oldData = new KyThi
            {
                Id = existingKyThi.Id,
                TenKyThi = existingKyThi.TenKyThi,
                NgayThi = existingKyThi.NgayThi,
                GioBatDau = existingKyThi.GioBatDau,
                GioKetThuc = existingKyThi.GioKetThuc,
                ThoiLuong = existingKyThi.ThoiLuong,
                LopHocId = existingKyThi.LopHocId,
                TrangThai = existingKyThi.TrangThai
            };

            var result = await _service.UpdateAsync(id, request);
            if (result == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Cập nhật kỳ thi thất bại"
                });
            }
            await _logService.LogUpdateAsync(GetCurrentUserId(), oldData, result, GetClientIpAddress());

            return Ok(new
            {
                success = true,
                message = "Cập nhật kỳ thi thành công",
                data = new KyThiResponse
                {
                    Id = existingKyThi.Id,
                    TenKyThi = existingKyThi.TenKyThi,
                    NgayThi = existingKyThi.NgayThi,
                    GioBatDau = existingKyThi.GioBatDau,
                    GioKetThuc = existingKyThi.GioKetThuc,
                    ThoiLuong = existingKyThi.ThoiLuong,
                    LopHocId = existingKyThi.LopHocId,
                    TrangThai = existingKyThi.TrangThai
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi cập nhật kỳ thi",
                error = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var kyThi = await _service.GetByIdAsync(id);
            if (kyThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kỳ thi"
                });
            }

            await _service.DeleteAsync(id);

            return Ok(new
            {
                success = true,
                message = "Xóa kỳ thi thành công"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi xóa kỳ thi",
                error = ex.Message
            });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateStatus(string id, [FromBody] TrangThaiKyThi trangThai)
    {
        try
        {
            var kyThi = await _service.GetByIdAsync(id);
            if (kyThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kỳ thi"
                });
            }

            var request = new KyThiUpdateRequest
            {
                TrangThai = trangThai
            };
            await _service.UpdateAsync(id, request);

            return Ok(new
            {
                success = true,
                message = "Cập nhật trạng thái kỳ thi thành công",
                data = new KyThiResponse
                {
                    Id = kyThi.Id,
                    TrangThai = kyThi.TrangThai
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi cập nhật trạng thái",
                error = ex.Message
            });
        }
    }

    [HttpGet("lop/{lopHocId}")]
    public async Task<ActionResult> GetByLopHoc(string lopHocId)
    {
        try
        {
            var kyThis = await _service.GetByClassIdAsync(lopHocId);

            return Ok(new
            {
                success = true,
                data = kyThis.Select(kt => new KyThiResponse
                {
                    Id = kt.Id,
                    TenKyThi = kt.TenKyThi,
                    NgayThi = kt.NgayThi,
                    ThoiLuong = kt.ThoiLuong,
                    TrangThai = kt.TrangThai
                }),
                total = kyThis.Count()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy danh sách kỳ thi",
                error = ex.Message
            });
        }
    }
}


