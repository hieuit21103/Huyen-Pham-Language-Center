using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.PhienLamBai;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
public class PhienLamBaiController : BaseController<PhienLamBai>
{
    private readonly ITestSessionService _service;
    public PhienLamBaiController(
        ITestSessionService service,
        ISystemLoggerService logService
        ) : base(logService)
    {
        _service = service;
    }

    [HttpPost("submit")]
    [Authorize]
    public async Task<ActionResult> Submit([FromBody] SubmitRequest request)
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

            var createdPhienLamBai = await _service.SubmitAsync(request, taiKhoanId: Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""));
            if (createdPhienLamBai == null)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Không thể nộp bài thi" 
                });
            }

            return Ok(new
            {
                success = true,
                message = "Nộp bài thành công",
                data = createdPhienLamBai
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi nộp bài",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Chấm bài thi (dành cho giáo viên)
    /// </summary>
    /// <param name="id">ID Phiên làm bài</param>
    /// <param name="request">Điểm và nhận xét cho từng câu</param>
    /// <returns>Bài thi đã chấm với tổng điểm</returns>
    /// <response code="200">Chấm bài thành công</response>
    /// <response code="404">Không tìm thấy bài thi</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Tổng điểm sẽ được tự động tính từ tổng điểm các câu hỏi
    /// </remarks>
    [HttpPost("{id}/grade")]
    [Authorize(Roles = "giaovien,giaovu,admin")]
    public async Task<ActionResult> Grade(string id, [FromBody] GradingRequest request)
    {
        try
        {
            var phienLamBai = await _service.GetByIdAsync(id);
            if (phienLamBai == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bài thi"
                });
            }
            phienLamBai.SoCauDung = request.SoCauDung;
            phienLamBai.Diem = request.Diem;

            await _service.GradeAsync(id, request);

            return Ok(new
            {
                success = true,
                message = "Chấm bài thành công",
                data = phienLamBai
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi chấm bài",
                error = ex.Message
            });
        }
    }

    [HttpGet("{id}/details")]
    [Authorize(Roles = "hocvien,giaovien,giaovu,admin")]
    public async Task<ActionResult> GetDetails(string id)
    {
        try
        {
            var phienLamBai = await _service.GetByIdAsync(id);
            if (phienLamBai == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bài thi"
                });
            }

            var result = await _service.GetDetailsAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy chi tiết bài thi"
                });
            }

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy chi tiết bài thi",
                error = ex.Message
            });
        }
    }

    [HttpGet("hocvien/{hocVienId}")]
    public async Task<ActionResult> GetByHocVien(string hocVienId)
    {
        try
        {
            var result = await _service.GetByHocVienIdAsync(hocVienId);

            return Ok(new
            {
                success = true,
                data = result.Select(plb => new
                {
                    id = plb.Id,
                    deThiId = plb.DeThiId,
                    deThi = plb.DeThi,
                    diem = plb.Diem,
                    ngayLam = plb.NgayLam,
                }),
                count = result.Count()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy danh sách bài thi",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy danh sách bài thi theo đề thi
    /// </summary>
    /// <param name="deThiId">ID đề thi</param>
    /// <returns>Danh sách bài thi đã nộp cho đề thi này</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("dethi/{deThiId}")]
    public async Task<ActionResult> GetByDeThi(string deThiId)
    {
        try
        {
            var result = await _service.GetByDeThiIdAsync(deThiId);

            return Ok(new
            {
                success = true,
                data = result.Select(plb => new
                {
                    id = plb.Id,
                    hocVienId = plb.HocVienId,
                    diem = plb.Diem,
                    ngayLam = plb.NgayLam,
                    soCauDung = plb.SoCauDung
                }),
                total = result.Count()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy danh sách bài thi",
                error = ex.Message
            });
        }
    }
}


