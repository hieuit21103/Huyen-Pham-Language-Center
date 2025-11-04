using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.BaiThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller.Learning;

/// <summary>
/// Controller quản lý bài thi (nộp bài, chấm bài)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PhienLamBaiController : BaseController<PhienLamBai>
{
    private readonly IGenericService<CauTraLoi> _cauTraLoiService;
    private readonly IGenericService<NganHangCauHoi> _cauHoiService;
    private readonly IGenericService<DapAnCauHoi> _dapAnCauHoiService;
    private readonly IGenericService<HocVien> _hocVienService;
    private readonly IGenericService<DeThi> _deThiService;

    public PhienLamBaiController(
        IGenericService<PhienLamBai> service,
        IGenericService<CauTraLoi> cauTraLoiService,
        IGenericService<NganHangCauHoi> cauHoiService,
        IGenericService<DapAnCauHoi> dapAnCauHoiService,
        IGenericService<HocVien> hocVienService,
        IGenericService<DeThi> deThiService
        ) : base(service)
    {
        _cauTraLoiService = cauTraLoiService;
        _cauHoiService = cauHoiService;
        _dapAnCauHoiService = dapAnCauHoiService;
        _hocVienService = hocVienService;
        _deThiService = deThiService;
    }

    /// <summary>
    /// Nộp bài thi
    /// </summary>
    /// <param name="request">
    /// Thông tin bài thi và câu trả lời
    /// </param>
    /// <returns>Bài thi đã nộp</returns>
    /// <response code="200">Nộp bài thành công</response>
    /// <response code="404">Không tìm thấy đề thi</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    /// <remarks>
    /// Hệ thống sẽ tự động chấm điểm cho các câu trắc nghiệm
    /// </remarks>
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

            var taiKhoanId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var hocVien = await _hocVienService.GetAllAsync(
                PageNumber: 1,
                PageSize: 1,
                Filter: hv => hv.TaiKhoanId.ToString() == taiKhoanId
            );
            Console.WriteLine(hocVien.FirstOrDefault());
            if (hocVien.Count() == 0)
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Không tìm thấy học viên" 
                });
            }

            var thoiGianLamBai = TimeSpan.FromSeconds(request.ThoiGianLamBai);
            var soCauDung = 0;
            var diemTong = 0;

            if (request.TuDongCham == false)
            {
                diemTong = -1;
                soCauDung = -1;
            }
            else
            {
                foreach (var answer in request.CacTraLoi)
                {
                    var cauHoiId = answer.Key;
                    var cauTraLoi = answer.Value;
                    var isCorrect = IsTrueAnswer(cauHoiId, cauTraLoi);
                    if (isCorrect)
                    {
                        soCauDung++;
                        var dapAn = await _dapAnCauHoiService.GetAllAsync(
                            PageNumber: 1,
                            PageSize: 1,
                            Filter: da => da.CauHoiId == cauHoiId && da.Dung == true
                        );
                        diemTong++;
                    }
                }
            }

            var baiThi = new PhienLamBai
            {
                TongCauHoi = request.TongCauHoi,
                SoCauDung = soCauDung == -1 ? null : soCauDung,
                Diem = diemTong == -1 ? null : diemTong,
                ThoiGianLam = thoiGianLamBai,
                HocVienId = hocVien.FirstOrDefault()!.Id,
                DeThiId = Guid.Parse(request.DeThiId),
            };

            var createdPhienLamBai = await _service.AddAsync(baiThi);
            if (createdPhienLamBai == null)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Không thể nộp bài thi" 
                });
            }

            foreach (var answer in request.CacTraLoi)
            {
                var cauTraLoi = new CauTraLoi
                {
                    PhienId = createdPhienLamBai.Id,
                    CauHoiId = answer.Key,
                    CauTraLoiText = answer.Value,
                    Dung = IsTrueAnswer(answer.Key, answer.Value)
                };
                var result = await _cauTraLoiService.AddAsync(cauTraLoi);
                if (result == null)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Không thể lưu câu trả lời" 
                    });
                }
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

            await _service.UpdateAsync(phienLamBai);

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

    /// <summary>
    /// Lấy chi tiết bài thi và các câu trả lời
    /// </summary>
    /// <param name="id">ID bài thi</param>
    /// <returns>Thông tin bài thi và danh sách câu trả lời</returns>
    /// <response code="200">Lấy chi tiết thành công</response>
    /// <response code="404">Không tìm thấy bài thi</response>
    /// <response code="500">Lỗi server</response>
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

            var hocVien = await _hocVienService.GetByIdAsync(phienLamBai.HocVienId.ToString());

            if (hocVien.TaiKhoan.Id.ToString() != User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                && !User.IsInRole("giaovien") && !User.IsInRole("giaovu") && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            var cauTraLoi = await _cauTraLoiService.GetAllAsync(
                PageNumber: 1,
                PageSize: int.MaxValue,
                Filter: ct => ct.PhienId == phienLamBai.Id,
                Includes: ct => ct.CauHoi
            );

            return Ok(new
            {
                success = true,
                data = new
                {
                    phienLamBai = new
                    {
                        id = phienLamBai.Id,
                        tongCauHoi = phienLamBai.TongCauHoi,
                        soCauDung = phienLamBai.SoCauDung,
                        diem = phienLamBai.Diem,
                        thoiGianLam = phienLamBai.ThoiGianLam,
                        ngayLam = phienLamBai.NgayLam,
                        deThiId = phienLamBai.DeThiId,
                        hocVienId = phienLamBai.HocVienId
                    },
                    cauTraLoi = cauTraLoi.Select(ct => new
                    {
                        id = ct.Id,
                        cauHoiId = ct.CauHoiId,
                        cacDapAn = ct.CauHoi.CacDapAn.Select(da => new
                        {
                            id = da.Id,
                            nhan = da.Nhan,
                            noiDung = da.NoiDung,
                            dung = da.Dung,
                            giaiThich = da.GiaiThich
                        }),
                        cauTraLoiText = ct.CauTraLoiText,
                        dung = ct.Dung,
                        noiDungCauHoi = ct.CauHoi.NoiDungCauHoi
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
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Danh sách bài thi</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("hocvien/{hocVienId}")]
    public async Task<ActionResult> GetByHocVien(
        string hocVienId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        try
        {
            var phienLamBai = await _service.GetAllAsync(
                PageNumber: pageNumber,
                PageSize: pageSize,
                Filter: plb => plb.HocVienId == Guid.Parse(hocVienId)
            );

            return Ok(new
            {
                success = true,
                data = phienLamBai.Select(plb => new
                {
                    id = plb.Id,
                    deThiId = plb.DeThiId,
                    deThi = plb.DeThi,
                    diem = plb.Diem,
                    ngayLam = plb.NgayLam,
                }),
                count = phienLamBai.Count()
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
            var phienLamBai = await _service.GetAllAsync(
                PageNumber: 1,
                PageSize: 100,
                Filter: plb => plb.DeThiId.ToString() == deThiId
            );

            return Ok(new
            {
                success = true,
                data = phienLamBai.Select(plb => new
                {
                    id = plb.Id,
                    hocVienId = plb.HocVienId,
                    diem = plb.Diem,
                    ngayLam = plb.NgayLam,
                    soCauDung = plb.SoCauDung
                }),
                total = phienLamBai.Count()
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

    private bool IsTrueAnswer(Guid cauHoiId, string cauTraLoi)
    {
        var dapAn = _dapAnCauHoiService.GetAllAsync(
            PageNumber: 1,
            PageSize: 1,
            Filter: da => da.CauHoiId == cauHoiId && da.Dung == true
        ).Result.FirstOrDefault();

        if (dapAn != null && dapAn.Nhan.Equals(cauTraLoi, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        return false;
    }
}
