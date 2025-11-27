using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
public class DeThiController : BaseController
{
    private readonly IExamService _examService;
    
    public DeThiController(
        ISystemLoggerService loggerService,
        IExamService examService) : base(loggerService)
    {
        _examService = examService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var deThis = await _examService.GetAllAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đề thi thành công",
            data = deThis
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var deThi = await _examService.GetByIdAsync(id);
        if (deThi == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy đề thi"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Lấy thông tin đề thi thành công",
            data = deThi
        });
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<IActionResult> GetTestsByCreator(string creatorId)
    {
        if (!Guid.TryParse(creatorId, out var nguoiTaoId))
        {
            return BadRequest(new { success = false, message = "ID người tạo không hợp lệ" });
        }

        var deThis = await _examService.GetByCreatorAsync(nguoiTaoId);
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đề thi theo người tạo thành công",
            data = deThis
        });
    }

    [HttpGet("exam/{examId}")]
    public async Task<IActionResult> GetTestsByExam(string examId)
    {
        if (!Guid.TryParse(examId, out var kyThiId))
        {
            return BadRequest(new { success = false, message = "ID kỳ thi không hợp lệ" });
        }

        var deThis = await _examService.GetByExamSessionAsync(kyThiId);
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đề thi theo kỳ thi thành công",
            data = deThis
        });
    }

    [HttpPost("generate")]
    public async Task<ActionResult> GenerateFromConfig([FromBody] GenerateExamRequest request)
    {
        try
        {
            request.NguoiTaoId = GetCurrentUserId();
            var deThi = await _examService.GenerateExamAsync(request);

            return Ok(new
            {
                success = true,
                message = "Tạo đề thi tự động thành công",
                data = new
                {
                    id = deThi.Id,
                    tenDe = deThi.TenDe,
                    maDe = deThi.MaDe,
                    thoiLuongPhut = deThi.ThoiLuongPhut,
                    tongSoCau = deThi.CacCauHoi.Count,
                    kyThiId = deThi.KyThiId,
                    ngayTao = deThi.NgayTao
                }
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message,
                error = "NOT_FOUND"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                error = "INSUFFICIENT_QUESTIONS_OR_GROUPS"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi tạo đề thi tự động",
                error = ex.Message
            });
        }
    }

    [HttpPost("generate-practice")]
    public async Task<ActionResult> GeneratePracticeTest([FromBody] GeneratePracticeTestRequest request)
    {
        try
        {
            var nguoiTaoId = GetCurrentUserId();
            var deThi = await _examService.GeneratePracticeTestAsync(
                request.CapDo,
                request.DoKho,
                request.KyNang,
                request.SoCauHoi,
                request.ThoiLuongPhut,
                nguoiTaoId,
                request.CheDoCauHoi
            );

            return Ok(new
            {
                success = true,
                message = "Tạo đề luyện tập thành công",
                data = new
                {
                    id = deThi.Id,
                    tenDe = deThi.TenDe,
                    maDe = deThi.MaDe,
                    thoiLuongPhut = deThi.ThoiLuongPhut,
                    tongSoCau = deThi.CacCauHoi.Count,
                    ngayTao = deThi.NgayTao
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                error = "INSUFFICIENT_QUESTIONS_OR_GROUPS"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi tạo đề luyện tập",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<DeThi>> Create([FromBody] DeThiRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            request.NguoiTaoId = GetCurrentUserId();
            var createdDeThi = await _examService.CreateAsync(request);

            return Ok(new
            {
                success = true,
                message = "Tạo đề thi thành công",
                data = new
                {
                    id = createdDeThi.Id,
                    tenDe = createdDeThi.TenDe,
                    thoiGianLamBai = createdDeThi.ThoiLuongPhut,
                    kyThiId = createdDeThi.KyThiId
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi tạo đề thi",
                error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DeThi>> Update(string id, [FromBody] DeThiUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedDeThi = await _examService.UpdateAsync(id, request);
            if (updatedDeThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật đề thi thành công",
                data = new
                {
                    id = updatedDeThi.Id,
                    tenDe = updatedDeThi.TenDe,
                    thoiGianLamBai = updatedDeThi.ThoiLuongPhut,
                    kyThiId = updatedDeThi.KyThiId
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi cập nhật đề thi",
                error = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var deleted = await _examService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa đề thi thành công"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi xóa đề thi",
                error = ex.Message
            });
        }
    }

    [HttpGet("{id}/questions")]
    public async Task<ActionResult> GetQuestionsInTest(string id)
    {
        try
        {
            var deThi = await _examService.GetWithQuestionsAsync(id);

            if (deThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            var questions = deThi.CacCauHoi
                .Where(chdt => chdt.CauHoi != null)
                .OrderBy(chdt => chdt.ThuTuCauHoi)
                .Select(chdt => chdt.CauHoi);

            return Ok(new
            {
                success = true,
                data = questions,
                totalQuestions = deThi.CacCauHoi.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy danh sách câu hỏi",
                error = ex.Message
            });
        }
    }

    [HttpGet("{id}/questions-grouped")]
    public async Task<ActionResult> GetQuestionsGrouped(string id)
    {
        try
        {
            var deThi = await _examService.GetWithQuestionsAsync(id);

            if (deThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            var groupedQuestions = deThi.CacCauHoi
                .Where(chdt => chdt.NhomCauHoiId.HasValue && chdt.CauHoi != null)
                .OrderBy(chdt => chdt.ThuTuCauHoi)
                .Select(chdt => new
                {
                    nhomCauHoiId = chdt.NhomCauHoiId,
                    cauHoi = chdt.CauHoi,
                    thuTu = chdt.ThuTuCauHoi
                });

            var standaloneQuestions = deThi.CacCauHoi
                .Where(chdt => !chdt.NhomCauHoiId.HasValue && chdt.CauHoi != null)
                .OrderBy(chdt => chdt.ThuTuCauHoi)
                .Select(chdt => new
                {
                    cauHoi = chdt.CauHoi,
                    thuTu = chdt.ThuTuCauHoi
                });

            return Ok(new
            {
                success = true,
                data = new
                {
                    deThi = new
                    {
                        id = deThi.Id,
                        tenDe = deThi.TenDe,
                        maDe = deThi.MaDe,
                        thoiLuongPhut = deThi.ThoiLuongPhut
                    },
                    groupedQuestions = groupedQuestions,
                    standaloneQuestions = standaloneQuestions,
                    totalQuestions = deThi.CacCauHoi.Count
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy danh sách câu hỏi",
                error = ex.Message
            });
        }
    }
}


