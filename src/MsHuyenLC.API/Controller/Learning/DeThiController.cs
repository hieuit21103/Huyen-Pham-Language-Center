using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Services.Learnings;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.API.Controller.Learning;

/// <summary>
/// Controller quản lý đề thi
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DeThiController : BaseController<DeThi>
{
    private readonly TestService _testService;
    private readonly IGenericService<NganHangCauHoi> _cauHoiService;
    private readonly IGenericService<CauHoiDeThi> _cauHoiDeThiService;

    public DeThiController(
        IGenericService<DeThi> service,
        TestService testService,
        IGenericService<NganHangCauHoi> cauHoiService,
        IGenericService<CauHoiDeThi> cauHoiDeThiService) : base(service)
    {
        _testService = testService;
        _cauHoiService = cauHoiService;
        _cauHoiDeThiService = cauHoiDeThiService;
    }

    /// <summary>
    /// Tự động tạo đề thi với câu hỏi ngẫu nhiên
    /// </summary>
    /// <param name="request">Tiêu chí tạo đề thi (loại câu hỏi, kỹ năng, cấp độ, độ khó)</param>
    /// <returns>Đề thi đã được tạo</returns>
    /// <response code="200">Tạo đề thi thành công</response>
    /// <response code="400">Không đủ câu hỏi hoặc dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Hệ thống sẽ tự động chọn ngẫu nhiên câu hỏi từ ngân hàng đề theo các tiêu chí đã chọn
    /// </remarks>
    [HttpPost("generate")]
    public async Task<ActionResult> GenerateTest([FromBody] GenerateTestRequest request)
    {
        try
        {
            var deThi = await _testService.GenerateTestAsync(
                request.TenDe,
                request.SoCauHoi,
                request.ThoiGianLamBai,
                request.LoaiDeThi,
                request.LoaiCauHoi,
                request.KyNang,
                request.CapDo,
                request.DoKho,
                GetCurrentUserId()
            );

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

    /// <summary>
    /// Tạo đề thi với phân bổ câu hỏi theo độ khó
    /// </summary>
    /// <param name="request">Tiêu chí và số lượng câu theo từng độ khó (dễ/trung bình/khó)</param>
    /// <returns>Đề thi đã được tạo</returns>
    /// <response code="200">Tạo đề thi thành công</response>
    /// <response code="400">Không đủ câu hỏi hoặc dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Cho phép tạo đề thi với số lượng câu hỏi cụ thể cho từng mức độ khó
    /// </remarks>
    [HttpPost("generate-with-difficulty")]
    public async Task<ActionResult> GenerateTestWithDifficulty([FromBody] GenerateTestWithDifficultyRequest request)
    {
        try
        {
            var deThi = await _testService.GenerateTestWithDifficultyDistributionAsync(
                request.TenDe,
                request.SoCauDe,
                request.SoCauTrungBinh,
                request.SoCauKho,
                request.ThoiGianLamBai,
                request.LoaiDeThi,
                request.LoaiCauHoi,
                request.KyNang,
                request.CapDo,
                GetCurrentUserId()
            );

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

    /// <summary>
    /// Tạo đề thi thủ công với danh sách câu hỏi được chọn
    /// </summary>
    /// <param name="request">Thông tin đề thi và danh sách ID câu hỏi</param>
    /// <returns>Đề thi đã tạo</returns>
    /// <response code="200">Tạo đề thi thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Số câu hỏi phải khớp với số lượng ID câu hỏi được cung cấp
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<DeThi>> Create([FromBody] DeThiRequest request)
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
                    message = "KyThiId không được để trống khi tạo đề thi chính thức"
                });
            }

            if (request.SoCauHoi != request.CauHoiIds.Count)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Số câu hỏi ({request.SoCauHoi}) không khớp với số lượng câu hỏi được chọn ({request.CauHoiIds.Count})"
                });
            }

            // Validate câu hỏi tồn tại
            var cauHoi = new List<NganHangCauHoi>();
            foreach (var cauHoiId in request.CauHoiIds)
            {
                var ch = await _cauHoiService.GetByIdAsync(cauHoiId);
                if (ch == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Câu hỏi với ID {cauHoiId} không tồn tại"
                    });
                }
                cauHoi.Add(ch);
            }

            // Tạo đề thi
            var deThi = new DeThi
            {
                TenDe = request.TenDe,
                TongCauHoi = request.SoCauHoi,
                ThoiLuongPhut = request.ThoiGianLamBai,
                LoaiDeThi = request.LoaiDeThi,
                KyThiId = request.KyThiId,
                NguoiTaoId = GetCurrentUserId()
            };

            var createdDeThi = await _testService.AddAsync(deThi);

            // Thêm câu hỏi vào đề thi
            foreach (var ch in cauHoi)
            {
                var cauHoiDeThi = new CauHoiDeThi
                {
                    DeThiId = createdDeThi.Id,
                    CauHoiId = ch.Id
                };
                await _cauHoiDeThiService.AddAsync(cauHoiDeThi);
            }

            return Ok( new
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

            // Kiểm tra đề thi tồn tại
            var existingDeThi = await _testService.GetByIdAsync(id);
            if (existingDeThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
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

            if (request.SoCauHoi != request.CauHoiIds.Count)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Số câu hỏi ({request.SoCauHoi}) không khớp với số lượng câu hỏi được chọn ({request.CauHoiIds.Count})"
                });
            }

            // Validate câu hỏi tồn tại
            var cauHoi = new List<NganHangCauHoi>();
            foreach (var cauHoiId in request.CauHoiIds)
            {
                var ch = await _cauHoiService.GetByIdAsync(cauHoiId);
                if (ch == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Câu hỏi với ID {cauHoiId} không tồn tại"
                    });
                }
                cauHoi.Add(ch);
            }

            existingDeThi.TenDe = request.TenDe;
            existingDeThi.TongCauHoi = request.SoCauHoi;
            existingDeThi.ThoiLuongPhut = request.ThoiGianLamBai;
            existingDeThi.LoaiDeThi = request.LoaiDeThi;
            existingDeThi.KyThiId = request.KyThiId;

            await _testService.UpdateAsync(existingDeThi);

            var oldCauHoiDeThis = await _cauHoiDeThiService.GetAllAsync(
                PageNumber: 1,
                PageSize: 1000,
                Filter: cd => cd.DeThiId == existingDeThi.Id
            );

            foreach (var old in oldCauHoiDeThis)
            {
                await _cauHoiDeThiService.DeleteAsync(old);
            }

            foreach (var ch in cauHoi)
            {
                var cauHoiDeThi = new CauHoiDeThi
                {
                    DeThiId = existingDeThi.Id,
                    CauHoiId = ch.Id
                };
                await _cauHoiDeThiService.AddAsync(cauHoiDeThi);
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật đề thi thành công",
                data = new
                {
                    id = existingDeThi.Id,
                    tenDe = existingDeThi.TenDe,
                    soCauHoi = existingDeThi.TongCauHoi,
                    thoiGianLamBai = existingDeThi.ThoiLuongPhut,
                    loaiDeThi = existingDeThi.LoaiDeThi,
                    kyThiId = existingDeThi.KyThiId
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
            var deThi = await _testService.GetByIdAsync(id);

            if (deThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            await _testService.DeleteAsync(deThi);

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
    /// Lấy danh sách câu hỏi trong đề thi
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
            var deThi = await _testService.GetTestWithQuestionsAsync(id);

            if (deThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            var questions = _testService.GetQuestionsInTest(deThi);

            return Ok(new
            {
                success = true,
                data = questions,
                totalQuestions = _testService.GetNumberOfQuestions(deThi)
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
