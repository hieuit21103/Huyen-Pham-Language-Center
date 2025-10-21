using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.BaiThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.API.Controller.Learning;

/// <summary>
/// Controller quản lý bài thi (nộp bài, chấm bài)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BaiThiController : BaseController<BaiThi>
{
    private readonly IGenericService<BaiThiChiTiet> _baiThiChiTietService;
    private readonly IGenericService<CauHoi> _cauHoiService;
    private readonly IGenericService<DeThi> _deThiService;

    public BaiThiController(
        IGenericService<BaiThi> service,
        IGenericService<BaiThiChiTiet> baiThiChiTietService,
        IGenericService<CauHoi> cauHoiService,
        IGenericService<DeThi> deThiService) : base(service)
    {
        _baiThiChiTietService = baiThiChiTietService;
        _cauHoiService = cauHoiService;
        _deThiService = deThiService;
    }

    /// <summary>
    /// Nộp bài thi
    /// </summary>
    /// <param name="request">Thông tin bài thi và câu trả lời</param>
    /// <returns>Bài thi đã nộp</returns>
    /// <response code="200">Nộp bài thành công</response>
    /// <response code="404">Không tìm thấy đề thi</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Hệ thống sẽ tự động chấm điểm cho các câu trắc nghiệm
    /// </remarks>
    [HttpPost("submit")]
    public async Task<ActionResult> SubmitBaiThi([FromBody] SubmitBaiThiRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra đề thi tồn tại
            var deThi = await _deThiService.GetByIdAsync(request.DeThiId.ToString());
            if (deThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đề thi"
                });
            }

            // Tạo bài thi
            var baiThi = new BaiThi
            {
                DeThiId = request.DeThiId,
                HocVienId = request.HocVienId,
                NgayNop = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var createdBaiThi = await _service.AddAsync(baiThi);

            // Lưu chi tiết câu trả lời
            foreach (var cauTraLoi in request.CauTraLois)
            {
                var baiThiChiTiet = new BaiThiChiTiet
                {
                    BaiThiId = createdBaiThi.Id,
                    CauHoiId = cauTraLoi.CauHoiId,
                    CauTraLoi = cauTraLoi.CauTraLoi
                };

                await _baiThiChiTietService.AddAsync(baiThiChiTiet);
            }

            // Tự động chấm bài trắc nghiệm (nếu có)
            await AutoGradeTracNghiem(createdBaiThi.Id);

            return Ok(new
            {
                success = true,
                message = "Nộp bài thành công",
                data = new
                {
                    baiThiId = createdBaiThi.Id,
                    ngayNop = createdBaiThi.NgayNop
                }
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
    /// <param name="id">ID bài thi</param>
    /// <param name="request">Điểm và nhận xét cho từng câu</param>
    /// <returns>Bài thi đã chấm với tổng điểm</returns>
    /// <response code="200">Chấm bài thành công</response>
    /// <response code="404">Không tìm thấy bài thi</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Tổng điểm sẽ được tự động tính từ tổng điểm các câu hỏi
    /// </remarks>
    [HttpPost("{id}/cham")]
    public async Task<ActionResult> ChamBai(string id, [FromBody] ChamBaiRequest request)
    {
        try
        {
            var baiThi = await _service.GetByIdAsync(id);
            if (baiThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bài thi"
                });
            }

            // Chấm từng câu hỏi
            foreach (var chamCauHoi in request.CauHois)
            {
                var baiThiChiTiet = await _baiThiChiTietService.GetByIdAsync(chamCauHoi.BaiThiChiTietId.ToString());
                if (baiThiChiTiet != null)
                {
                    baiThiChiTiet.Diem = chamCauHoi.Diem;
                    baiThiChiTiet.NhanXet = chamCauHoi.NhanXet;
                    await _baiThiChiTietService.UpdateAsync(baiThiChiTiet);
                }
            }

            // Tính tổng điểm
            var allChiTiet = await _baiThiChiTietService.GetAllAsync(
                PageNumber: 1,
                PageSize: 1000,
                Filter: ct => ct.BaiThiId == baiThi.Id
            );

            var tongDiem = allChiTiet.Sum(ct => ct.Diem ?? 0);
            baiThi.TongDiem = tongDiem;
            baiThi.NhanXet = request.NhanXetChung;

            await _service.UpdateAsync(baiThi);

            return Ok(new
            {
                success = true,
                message = "Chấm bài thành công",
                data = new
                {
                    baiThiId = baiThi.Id,
                    tongDiem = baiThi.TongDiem,
                    nhanXet = baiThi.NhanXet
                }
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

    /// <summary>
    /// Lấy chi tiết bài thi và các câu trả lời
    /// </summary>
    /// <param name="id">ID bài thi</param>
    /// <returns>Thông tin bài thi và danh sách câu trả lời</returns>
    /// <response code="200">Lấy chi tiết thành công</response>
    /// <response code="404">Không tìm thấy bài thi</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("{id}/chitiet")]
    public async Task<ActionResult> GetChiTiet(string id)
    {
        try
        {
            var baiThi = await _service.GetByIdAsync(id);
            if (baiThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bài thi"
                });
            }

            var chiTiets = await _baiThiChiTietService.GetAllAsync(
                PageNumber: 1,
                PageSize: 1000,
                Filter: ct => ct.BaiThiId == baiThi.Id
            );

            return Ok(new
            {
                success = true,
                data = new
                {
                    baiThi = new
                    {
                        id = baiThi.Id,
                        tongDiem = baiThi.TongDiem,
                        nhanXet = baiThi.NhanXet,
                        ngayNop = baiThi.NgayNop
                    },
                    chiTiets = chiTiets.Select(ct => new
                    {
                        id = ct.Id,
                        cauHoiId = ct.CauHoiId,
                        cauTraLoi = ct.CauTraLoi,
                        diem = ct.Diem,
                        nhanXet = ct.NhanXet
                    })
                }
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

    /// <summary>
    /// Lấy danh sách bài thi của học viên
    /// </summary>
    /// <param name="hocVienId">ID học viên</param>
    /// <returns>Danh sách bài thi</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("hocvien/{hocVienId}")]
    public async Task<ActionResult> GetByHocVien(string hocVienId)
    {
        try
        {
            var baiThis = await _service.GetAllAsync(
                PageNumber: 1,
                PageSize: 100,
                Filter: bt => bt.HocVienId.ToString() == hocVienId
            );

            return Ok(new
            {
                success = true,
                data = baiThis.Select(bt => new
                {
                    id = bt.Id,
                    deThiId = bt.DeThiId,
                    tongDiem = bt.TongDiem,
                    ngayNop = bt.NgayNop,
                    nhanXet = bt.NhanXet
                }),
                total = baiThis.Count()
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
            var baiThis = await _service.GetAllAsync(
                PageNumber: 1,
                PageSize: 100,
                Filter: bt => bt.DeThiId.ToString() == deThiId
            );

            return Ok(new
            {
                success = true,
                data = baiThis.Select(bt => new
                {
                    id = bt.Id,
                    hocVienId = bt.HocVienId,
                    tongDiem = bt.TongDiem,
                    ngayNop = bt.NgayNop,
                    nhanXet = bt.NhanXet
                }),
                total = baiThis.Count()
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

    private async Task AutoGradeTracNghiem(Guid baiThiId)
    {
        try
        {
            var chiTiets = await _baiThiChiTietService.GetAllAsync(
                PageNumber: 1,
                PageSize: 1000,
                Filter: ct => ct.BaiThiId == baiThiId
            );

            float diemTracNghiem = 0;

            foreach (var chiTiet in chiTiets)
            {
                var cauHoi = await _cauHoiService.GetByIdAsync(chiTiet.CauHoiId.ToString());
                
                if (cauHoi != null && cauHoi.LoaiCauHoi == LoaiCauHoi.TracNghiem)
                {
                    // So sánh đáp án
                    if (chiTiet.CauTraLoi?.Trim().ToLower() == cauHoi.DapAnDung?.Trim().ToLower())
                    {
                        chiTiet.Diem = 1; // Điểm cho mỗi câu trắc nghiệm đúng
                        diemTracNghiem += 1;
                    }
                    else
                    {
                        chiTiet.Diem = 0;
                    }

                    await _baiThiChiTietService.UpdateAsync(chiTiet);
                }
            }

            // Cập nhật điểm trắc nghiệm vào bài thi
            var baiThi = await _service.GetByIdAsync(baiThiId.ToString());
            if (baiThi != null)
            {
                baiThi.DiemTracNghiem = diemTracNghiem;
                baiThi.TongDiem = diemTracNghiem; // Tạm thời, sẽ cộng thêm điểm tự luận sau khi chấm
                await _service.UpdateAsync(baiThi);
            }
        }
        catch
        {
            // Log error but don't throw
        }
    }
}
