using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Application.Services.Learning;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
public class DeThiController : BaseController
{
    private readonly ITestService _service;
    public DeThiController(
        ISystemLoggerService loggerService,
        ITestService service) : base(loggerService)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy tất cả đề thi
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var deThis = await _service.GetAllAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đề thi thành công",
            data = deThis
        });
    }

    /// <summary>
    /// Lấy đề thi theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var deThi = await _service.GetByIdAsync(id);
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

    /// <summary>
    /// Lấy đề thi theo người tạo
    /// </summary>
    [HttpGet("creator/{creatorId}")]
    public async Task<IActionResult> GetTestsByCreator(string creatorId)
    {
        var deThis = await _service.GetTestsByCreatorAsync(creatorId);
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đề thi theo người tạo thành công",
            data = deThis
        });
    }

    /// <summary>
    /// Lấy đề thi theo kỳ thi
    /// </summary>
    [HttpGet("exam/{examId}")]
    public async Task<IActionResult> GetTestsByExam(string examId)
    {
        var deThis = await _service.GetTestsByExamAsync(examId);
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đề thi theo kỳ thi thành công",
            data = deThis
        });
    }

    /// <summary>
    /// Tự động tạo đề thi với câu hỏi ngẫu nhiên
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult> GenerateTest([FromBody] GenerateTestRequest request)
    {
        try
        {
            request.NguoiTaoId = GetCurrentUserId();
            var deThi = await _service.GenerateTestAsync(request);

            if (deThi == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không thể tạo đề thi"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Tạo đề thi thành công",
                data = new
                {
                    id = deThi.Id,
                    tenDe = deThi.TenDe,
                    soCauHoi = deThi.TongCauHoi,
                    thoiGianLamBai = deThi.ThoiLuongPhut,
                    loaiDeThi = deThi.LoaiDeThi,
                    kyThiId = deThi.KyThiId
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                error = "VALIDATION_ERROR"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                error = "INSUFFICIENT_QUESTIONS"
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

    [HttpPost("generate-with-difficulty")]
    public async Task<ActionResult> GenerateTestWithDifficulty([FromBody] GenerateTestWithDifficultyRequest request)
    {
        try
        {
            request.NguoiTaoId = GetCurrentUserId();
            var deThi = await _service.GenerateTestWithDifficultyDistributionAsync(request);

            return Ok(new
            {
                success = true,
                message = "Tạo đề thi theo phân bổ độ khó thành công",
                data = new
                {
                    id = deThi.Id,
                    tenDe = deThi.TenDe,
                    soCauHoi = deThi.TongCauHoi,
                    thoiGianLamBai = deThi.ThoiLuongPhut,
                    loaiDeThi = deThi.LoaiDeThi,
                    kyThiId = deThi.KyThiId
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                error = "VALIDATION_ERROR"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                error = "INSUFFICIENT_QUESTIONS"
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
            var createdDeThi = await _service.CreateAsync(request);

            return Ok(new
            {
                success = true,
                message = "Tạo đề thi thành công",
                data = new
                {
                    id = createdDeThi.Id,
                    tenDe = createdDeThi.TenDe,
                    soCauHoi = createdDeThi.TongCauHoi,
                    thoiGianLamBai = createdDeThi.ThoiLuongPhut,
                    loaiDeThi = createdDeThi.LoaiDeThi,
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

    [HttpPost("create-mixed")]
    public async Task<ActionResult> CreateMixed([FromBody] CreateMixedTestRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.LoaiDeThi == LoaiDeThi.ChinhThuc && string.IsNullOrEmpty(request.KyThiId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "KyThiId không được để trống khi tạo đề thi chính thức"
                });
            }

            var deThi = await _service.CreateMixedTestAsync(request);

            return Ok(new
            {
                success = true,
                message = "Tạo đề thi hỗn hợp thành công",
                data = new
                {
                    id = deThi.Id,
                    tenDe = deThi.TenDe,
                    soCauHoi = deThi.TongCauHoi,
                    thoiGianLamBai = deThi.ThoiLuongPhut,
                    loaiDeThi = deThi.LoaiDeThi,
                    kyThiId = deThi.KyThiId
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
        catch (InvalidOperationException ex)
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




    /// <summary>
    /// Cập nhật đề thi
    /// </summary>
    /// <param name="id">ID đề thi</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Đề thi đã cập nhật</returns>
    /// <response code="200">Cập nhật thành công</response>
    /// <response code="404">Không tìm thấy đề thi</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Sẽ xóa toàn bộ câu hỏi cũ và thêm câu hỏi mới theo request
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ActionResult<DeThi>> Update(string id, [FromBody] DeThiUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validation
            if (request.LoaiDeThi == LoaiDeThi.ChinhThuc && request.KyThiId == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "KyThiId không được để trống khi cập nhật đề thi chính thức"
                });
            }

            // Service sẽ xử lý validation và cập nhật
            var updatedDeThi = await _service.UpdateAsync(id, request);
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
                    soCauHoi = updatedDeThi.TongCauHoi,
                    thoiGianLamBai = updatedDeThi.ThoiLuongPhut,
                    loaiDeThi = updatedDeThi.LoaiDeThi,
                    kyThiId = updatedDeThi.KyThiId
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
            return NotFound(new
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
                message = "Có lỗi xảy ra khi cập nhật đề thi",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Xóa đề thi
    /// </summary>
    /// <param name="id">ID đề thi</param>
    /// <returns>Kết quả xóa</returns>
    /// <response code="200">Xóa thành công</response>
    /// <response code="404">Không tìm thấy đề thi</response>
    /// <response code="500">Lỗi server</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var deThi = await _service.GetByIdAsync(id);

            if (deThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            await _service.DeleteAsync(id);

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

    /// <summary>
    /// Lấy danh sách câu hỏi trong đề thi (flat list - deprecated, dùng /questions-grouped thay thế)
    /// </summary>
    /// <param name="id">ID đề thi</param>
    /// <returns>Danh sách câu hỏi và tổng số câu</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="404">Không tìm thấy đề thi</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("{id}/questions")]
    public async Task<ActionResult> GetQuestionsInTest(string id)
    {
        try
        {
            var deThi = await _service.GetTestWithQuestionsAsync(id);

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

    /// <summary>
    /// Lấy danh sách câu hỏi được nhóm theo NhomCauHoi (RECOMMENDED cho làm bài thi)
    /// </summary>
    /// <param name="id">ID đề thi</param>
    /// <returns>Danh sách nhóm câu hỏi với cấu trúc phân cấp</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="404">Không tìm thấy đề thi</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("{id}/questions-grouped")]
    public async Task<ActionResult> GetQuestionsGrouped(string id)
    {
        try
        {
            var result = await _service.GetTestWithQuestionsGroupedAsync(id);

            if (result == null)
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
                data = result
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


