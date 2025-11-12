using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class NhomCauHoiController : BaseController<NhomCauHoi>
{
    private readonly IGroupQuestionService _service;

    public NhomCauHoiController(
        ISystemLoggerService systemLoggerService,
        IGroupQuestionService service
        ) : base(systemLoggerService)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _service.GetAllAsync();
        return Ok(new
        {
            success = true,
            data = groups
        });
    }

    [HttpGet("{id}/questions")]
    public async Task<IActionResult> GetQuestions(string id)
    {
        var questions = await _service.GetQuestionsByGroupIdAsync(id);
        if (questions == null || !questions.Any())
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

        var result = await _service.CreateAsync(request);
        await _logService.LogCreateAsync(GetCurrentUserId(), result, GetClientIpAddress());
        
        return Ok(new
        {
            success = true,
            message = "Tạo nhóm câu hỏi thành công",
            data = result
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
    
        var result = await _service.AddQuestionToGroupAsync(id, request.cauHoiId, request.thuTu);
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

        var oldData = existingNhomCauHoi;
        var result = await _service.UpdateAsync(id, request);
        await _logService.LogUpdateAsync(GetCurrentUserId(), oldData, result, GetClientIpAddress());
        
        return Ok(new
        {
            success = true,
            message = "Cập nhật nhóm câu hỏi thành công",
            data = result
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

        await _service.DeleteAsync(id);
        await _logService.LogDeleteAsync(GetCurrentUserId(), existingNhomCauHoi, GetClientIpAddress());
        return Ok(new
        {
            success = true,
            message = "Xóa nhóm câu hỏi thành công"
        });
    }
}

