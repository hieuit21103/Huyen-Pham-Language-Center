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
public class CauHoiController : BaseController
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
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new
            {
                success = false,
                message = "ID không hợp lệ"
            });
        }

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
        var createdCauHois = new List<CauHoi>();
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
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new
            {
                success = false,
                message = "ID không hợp lệ"
            });
        }

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

        var oldData = new CauHoi
        {
            Id = existingCauHoi.Id,
            NoiDungCauHoi = existingCauHoi.NoiDungCauHoi,
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
        if (!Guid.TryParse(id, out _))
        {
            return BadRequest(new
            {
                success = false,
                message = "ID không hợp lệ"
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

        await _service.DeleteAsync(id);
        await _logService.LogDeleteAsync(GetCurrentUserId(), existingCauHoi, GetClientIpAddress());
        return Ok(new
        {
            success = true,
            message = "Xóa câu hỏi thành công"
        });
    }

    [HttpPost("bulk-delete")]
    public async Task<IActionResult> BulkDelete([FromBody] List<string> ids)
    {
        if (ids == null || !ids.Any())
        {
            return BadRequest(new
            {
                success = false,
                message = "Danh sách ID rỗng"
            });
        }

        var deletedCauHois = new List<CauHoi>();
        var invalidIds = new List<string>();

        foreach (var id in ids)
        {
            if (!Guid.TryParse(id, out _))
            {
                invalidIds.Add(id);
                continue;
            }

            var existingCauHoi = await _service.GetByIdAsync(id);
            if (existingCauHoi != null)
            {
                await _service.DeleteAsync(id);
                await _logService.LogDeleteAsync(GetCurrentUserId(), existingCauHoi, GetClientIpAddress());
                deletedCauHois.Add(existingCauHoi);
            }
        }

        if (invalidIds.Any() && !deletedCauHois.Any())
        {
            return BadRequest(new
            {
                success = false,
                message = "Tất cả ID không hợp lệ",
                invalidIds
            });
        }

        return Ok(new
        {
            success = true,
            message = $"Xóa thành công {deletedCauHois.Count}/{ids.Count} câu hỏi",
            data = deletedCauHois,
            invalidIds = invalidIds.Any() ? invalidIds : null
        });
    }

    [HttpGet("download-template")]
    public async Task<IActionResult> DownloadTemplate()
    {
        var memory = new MemoryStream();
        await _service.DownloadQuestionsTemplateAsync(memory);
        memory.Position = 0;
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = "CauHoi_Template.xlsx";
        return File(memory, contentType, fileName);
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportQuestions(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Vui lòng chọn file để import"
            });
        }

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) &&
            !file.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                success = false,
                message = "File phải có định dạng Excel (.xlsx hoặc .xls)"
            });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _service.ImportQuestionsAsync(stream);

            return Ok(new
            {
                success = true,
                message = $"Import thành công {result.ThanhCong}/{result.TongSo} câu hỏi",
                data = new
                {
                    total = result.TongSo,
                    success = result.ThanhCong,
                    failed = result.ThatBai,
                    errors = result.LoiChiTiet.Take(10)
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = $"Lỗi khi import: {ex.Message}"
            });
        }
    }
}

