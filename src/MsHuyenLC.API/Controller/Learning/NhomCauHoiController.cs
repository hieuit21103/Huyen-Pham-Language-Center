using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Application.Interfaces.System;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.Services.Learnings;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;
using System.Text.RegularExpressions;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class NhomCauHoiController : BaseController<NhomCauHoi>
{
    private readonly ISystemLoggerService _systemLoggerService;
    private readonly GroupQuestionService _groupQuestionService;
    public NhomCauHoiController(
        IGenericService<NhomCauHoi> service,
        ISystemLoggerService systemLoggerService,
        GroupQuestionService groupQuestionService
        ) : base(service)
    {
        _systemLoggerService = systemLoggerService;
        _groupQuestionService = groupQuestionService;
    }

    protected override Func<IQueryable<NhomCauHoi>, IOrderedQueryable<NhomCauHoi>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "capdo" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.CapDo))
                : (q => q.OrderBy(k => k.CapDo)),
            "dokho" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.DoKho))
                : (q => q.OrderBy(k => k.DoKho)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [HttpGet("{id}/questions")]
    public async Task<IActionResult> GetQuestions(string id)
    {
        var questions = await _groupQuestionService.GetQuestionsByGroupId(id);
        if (questions == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy câu hỏi nào trong nhóm"
            });
        }

        return Ok(new
        {
            success = true,
            data = questions
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NhomCauHoiRequest request)
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
        var nhomCauHoi = new NhomCauHoi
        {
            UrlHinhAnh = request.UrlHinhAnh,
            UrlAmThanh = request.UrlAmThanh,
            NoiDung = request.NoiDung,
            TieuDe = request.TieuDe,
            SoLuongCauHoi = request.SoLuongCauHoi,
            CapDo = request.CapDo,
            DoKho = request.DoKho,
        };
        var result = await _service.AddAsync(nhomCauHoi);
        await _systemLoggerService.LogCreateAsync(GetCurrentUserId(), nhomCauHoi, GetClientIpAddress());
        if (result == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Không thể tạo nhóm câu hỏi"
            });
        }
        return Ok(new
        {
            success = true,
            message = "Tạo nhóm câu hỏi thành công",
            data = nhomCauHoi
        });
    }

    [HttpPost("{id}/add-question")]
    public async Task<IActionResult> AddQuestion(string id,[FromBody] ThemCauHoiRequest request)
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
    
        var result = await _groupQuestionService.AddQuestionToGroup(id, request.cauHoiId, request.thuTu);
        if (result == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Không thể thêm câu hỏi vào nhóm"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Thêm câu hỏi vào nhóm thành công",
            data = result
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] NhomCauHoiUpdateRequest request)
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

        var existingNhomCauHoi = await _service.GetByIdAsync(id);
        if (existingNhomCauHoi == null)
        {
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy nhóm câu hỏi" 
            });
        }

        var oldData = new NhomCauHoi
        {
            Id = existingNhomCauHoi.Id,
            NoiDung = existingNhomCauHoi.NoiDung,
            TieuDe = existingNhomCauHoi.TieuDe,
            SoLuongCauHoi = existingNhomCauHoi.SoLuongCauHoi,
            UrlHinhAnh = existingNhomCauHoi.UrlHinhAnh,
            UrlAmThanh = existingNhomCauHoi.UrlAmThanh,
            CapDo = existingNhomCauHoi.CapDo,
            DoKho = existingNhomCauHoi.DoKho
        };

        if (!string.IsNullOrWhiteSpace(request.NoiDung))
            existingNhomCauHoi.NoiDung = request.NoiDung;

        if (!string.IsNullOrWhiteSpace(request.TieuDe))
            existingNhomCauHoi.TieuDe = request.TieuDe;

        if (request.SoLuongCauHoi.HasValue)
            existingNhomCauHoi.SoLuongCauHoi = request.SoLuongCauHoi.Value;

        if (!string.IsNullOrWhiteSpace(request.UrlHinhAnh))
            existingNhomCauHoi.UrlHinhAnh = request.UrlHinhAnh;

        if (!string.IsNullOrWhiteSpace(request.UrlAmThanh))
            existingNhomCauHoi.UrlAmThanh = request.UrlAmThanh;

        if (request.CapDo.HasValue)
            existingNhomCauHoi.CapDo = request.CapDo.Value;

        if (request.DoKho.HasValue)
            existingNhomCauHoi.DoKho = request.DoKho.Value;

        await _service.UpdateAsync(existingNhomCauHoi);
        await _systemLoggerService.LogUpdateAsync(GetCurrentUserId(), oldData, existingNhomCauHoi, GetClientIpAddress());
        return Ok(new
        {
            success = true,
            message = "Cập nhật nhóm câu hỏi thành công",
            data = existingNhomCauHoi
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingNhomCauHoi = await _service.GetByIdAsync(id);
        if (existingNhomCauHoi == null)
        {
            return NotFound(new 
            { 
                success = false, 
                message = "Không tìm thấy nhóm câu hỏi" 
            });
        }

        await _service.DeleteAsync(existingNhomCauHoi);
        await _systemLoggerService.LogDeleteAsync(GetCurrentUserId(), existingNhomCauHoi, GetClientIpAddress());
        return Ok(new
        {
            success = true,
            message = "Xóa nhóm câu hỏi thành công"
        });
    }
}