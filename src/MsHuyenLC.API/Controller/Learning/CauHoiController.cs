using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class CauHoiController : BaseController<NganHangCauHoi>
{
    private readonly IQuestionService _service;
    
    public CauHoiController(
        IQuestionService service,
        ISystemLoggerService loggerService
        ) : base(loggerService)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        var totalItems = await _service.CountAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách câu hỏi thành công",
            data = result,
            total = totalItems
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Câu hỏi không tồn tại"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Lấy câu hỏi thành công",
            data = result
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] List<CauHoiRequest> requests)
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

        if (requests == null || !requests.Any())
        {
            return BadRequest(new
            {
                success = false,
                message = "Danh sách câu hỏi rỗng"
            });
        }
        var createdCauHois = new List<NganHangCauHoi>();
        foreach (var request in requests)
        {
            var result = await _service.CreateAsync(request);
            if (result == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không thể tạo câu hỏi"
                });
            }
            await _logService.LogCreateAsync(GetCurrentUserId(), result, GetClientIpAddress());
            createdCauHois.Add(result);
        }

        return Ok(new
        {
            success = true,
            message = "Tạo câu hỏi thành công",
            data = createdCauHois
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CauHoiUpdateRequest request)
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

        var existingCauHoi = await _service.GetByIdAsync(id);
        if (existingCauHoi == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy câu hỏi"
            });
        }

        var oldData = new NganHangCauHoi
        {
            Id = existingCauHoi.Id,
            NoiDungCauHoi = existingCauHoi.NoiDungCauHoi,
            LoaiCauHoi = existingCauHoi.LoaiCauHoi,
            KyNang = existingCauHoi.KyNang,
            UrlHinhAnh = existingCauHoi.UrlHinhAnh,
            UrlAmThanh = existingCauHoi.UrlAmThanh,
            LoiThoai = existingCauHoi.LoiThoai,
            CapDo = existingCauHoi.CapDo,
            DoKho = existingCauHoi.DoKho
        };

        await _service.UpdateAsync(id, request);
        await _logService.LogUpdateAsync(GetCurrentUserId(), oldData, existingCauHoi, GetClientIpAddress());
        return Ok(new
        {
            success = true,
            message = "Cập nhật câu hỏi thành công",
            data = existingCauHoi
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingCauHoi = await _service.GetByIdAsync(id);
        if (existingCauHoi == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy câu hỏi"
            });
        }

        await _service.DeleteAsync(id);
        await _logService.LogDeleteAsync(GetCurrentUserId(), existingCauHoi, GetClientIpAddress());
        return Ok(new
        {
            success = true,
            message = "Xóa câu hỏi thành công"
        });
    }
}

