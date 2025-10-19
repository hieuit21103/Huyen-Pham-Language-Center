using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Services.Learnings;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
public class DeThiController : BaseController<DeThi>
{
    private readonly TestService _testService;
    private readonly IGenericService<CauHoi> _cauHoiService;
    private readonly IGenericService<CauHoiDeThi> _cauHoiDeThiService;

    public DeThiController(
        IGenericService<DeThi> service,
        TestService testService,
        IGenericService<CauHoi> cauHoiService,
        IGenericService<CauHoiDeThi> cauHoiDeThiService) : base(service)
    {
        _testService = testService;
        _cauHoiService = cauHoiService;
        _cauHoiDeThiService = cauHoiDeThiService;
    }

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
                request.DoKho
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
                    soCauHoi = deThi.SoCauHoi,
                    thoiGianLamBai = deThi.ThoiGianLamBai,
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
            var deThi = await _testService.GenerateTestWithDifficultyDistributionAsync(
                request.TenDe,
                request.SoCauDe,
                request.SoCauTrungBinh,
                request.SoCauKho,
                request.ThoiGianLamBai,
                request.LoaiDeThi,
                request.LoaiCauHoi,
                request.KyNang,
                request.CapDo
            );

            return Ok(new
            {
                success = true,
                message = "Tạo đề thi theo phân bổ độ khó thành công",
                data = new
                {
                    id = deThi.Id,
                    tenDe = deThi.TenDe,
                    soCauHoi = deThi.SoCauHoi,
                    thoiGianLamBai = deThi.ThoiGianLamBai,
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
            var cauHoi = new List<CauHoi>();
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
                SoCauHoi = request.SoCauHoi,
                ThoiGianLamBai = request.ThoiGianLamBai,
                LoaiDeThi = request.LoaiDeThi,
                KyThiId = request.KyThiId,
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
                    soCauHoi = createdDeThi.SoCauHoi,
                    thoiGianLamBai = createdDeThi.ThoiGianLamBai,
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

    [HttpPut("{id}")]
    public async Task<ActionResult<DeThi>> Update(string id, [FromBody] UpdateDeThiRequest request)
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
            var cauHoi = new List<CauHoi>();
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

            // Cập nhật thông tin đề thi
            existingDeThi.TenDe = request.TenDe;
            existingDeThi.SoCauHoi = request.SoCauHoi;
            existingDeThi.ThoiGianLamBai = request.ThoiGianLamBai;
            existingDeThi.LoaiDeThi = request.LoaiDeThi;
            existingDeThi.KyThiId = request.KyThiId;

            await _testService.UpdateAsync(existingDeThi);

            // Xóa các câu hỏi cũ
            var oldCauHoiDeThis = await _cauHoiDeThiService.GetAllAsync(
                PageNumber: 1,
                PageSize: 1000,
                Filter: cd => cd.DeThiId == existingDeThi.Id
            );

            foreach (var old in oldCauHoiDeThis)
            {
                await _cauHoiDeThiService.DeleteAsync(old);
            }

            // Thêm câu hỏi mới
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
                    soCauHoi = existingDeThi.SoCauHoi,
                    thoiGianLamBai = existingDeThi.ThoiGianLamBai,
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

    [HttpGet("{id}/questions")]
    public async Task<ActionResult> GetQuestionsInTest(string id)
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
