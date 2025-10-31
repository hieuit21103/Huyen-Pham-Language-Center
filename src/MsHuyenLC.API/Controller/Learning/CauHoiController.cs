using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Application.Interfaces.System;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.Services.Learnings;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class CauHoiController : BaseController<NganHangCauHoi>
{
    private readonly ISystemLoggerService _systemLoggerService;
    public CauHoiController(
        QuestionService service,
        ISystemLoggerService systemLoggerService
        ) : base(service)
    {
        _systemLoggerService = systemLoggerService;
    }

    protected override Func<IQueryable<NganHangCauHoi>, IOrderedQueryable<NganHangCauHoi>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "loaicauhoi" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.LoaiCauHoi))
                : (q => q.OrderBy(k => k.LoaiCauHoi)),
            "kynang" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.KyNang))
                : (q => q.OrderBy(k => k.KyNang)),
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
            var dapAnCauHois = new List<DapAnCauHoi>();
            foreach (var dapAnRequest in request.DapAnCauHois ?? Array.Empty<DapAnRequest>())
            {
                if (string.IsNullOrWhiteSpace(dapAnRequest.Nhan) || string.IsNullOrWhiteSpace(dapAnRequest.NoiDung))
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Nhãn và nội dung đáp án không được để trống" 
                    });
                }
                dapAnCauHois.Add(new DapAnCauHoi
                {
                    Nhan = dapAnRequest.Nhan,
                    NoiDung = dapAnRequest.NoiDung,
                    Dung = dapAnRequest.Dung,
                    GiaiThich = dapAnRequest.GiaiThich
                });
            }

            var nhomCauHoiChiTiets = new List<NhomCauHoiChiTiet>();
            if (request.NhomCauHoiChiTiets != null && request.NhomCauHoiChiTiets.Any())
            {
                foreach (var nhomRequest in request.NhomCauHoiChiTiets)
                {
                    nhomCauHoiChiTiets.Add(new NhomCauHoiChiTiet
                    {
                        NhomId = nhomRequest.NhomId,
                        CauHoiId = nhomRequest.CauHoiId,
                        ThuTu = nhomRequest.ThuTu
                    });
                }
            }

            var cauHoi = new NganHangCauHoi
            {
                NoiDungCauHoi = request.NoiDungCauHoi,
                LoaiCauHoi = request.LoaiCauHoi,
                KyNang = request.KyNang,
                UrlHinhAnh = request.UrlHinhAnh,
                UrlAmThanh = request.UrlAmThanh,
                CapDo = request.CapDo,
                DoKho = request.DoKho,
                LoiThoai = request.LoiThoai,
                DoanVan = request.DoanVan,
                CacDapAn = dapAnCauHois
            };
            var result = await _service.AddAsync(cauHoi);
            await _systemLoggerService.LogCreateAsync(GetCurrentUserId(), cauHoi, GetClientIpAddress());
            if (result == null)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Không thể tạo câu hỏi" 
                });
            }
            createdCauHois.Add(cauHoi);
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
                DoanVan = existingCauHoi.DoanVan,
                CapDo = existingCauHoi.CapDo,
                DoKho = existingCauHoi.DoKho
            };

            if (request.NoiDungCauHoi != null)
                existingCauHoi.NoiDungCauHoi = request.NoiDungCauHoi;
            if (request.LoaiCauHoi.HasValue)
                existingCauHoi.LoaiCauHoi = request.LoaiCauHoi.Value;
            if (request.KyNang.HasValue)
                existingCauHoi.KyNang = request.KyNang.Value;
            if (request.UrlHinhAnh != null)
                existingCauHoi.UrlHinhAnh = request.UrlHinhAnh;
            if (request.UrlAmThanh != null)
                existingCauHoi.UrlAmThanh = request.UrlAmThanh;
            if (request.LoiThoai != null)
                existingCauHoi.LoiThoai = request.LoiThoai;
            if (request.DoanVan != null)
                existingCauHoi.DoanVan = request.DoanVan;
            if (request.CapDo.HasValue)
                existingCauHoi.CapDo = request.CapDo.Value;
            if (request.DoKho.HasValue)
                existingCauHoi.DoKho = request.DoKho.Value;

            await _service.UpdateAsync(existingCauHoi);
            await _systemLoggerService.LogUpdateAsync(GetCurrentUserId(), oldData, existingCauHoi, GetClientIpAddress());
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

            await _service.DeleteAsync(existingCauHoi);
            await _systemLoggerService.LogDeleteAsync(GetCurrentUserId(), existingCauHoi, GetClientIpAddress());
            return Ok(new
            {
                success = true,
                message = "Xóa câu hỏi thành công"
            });
        }
    }