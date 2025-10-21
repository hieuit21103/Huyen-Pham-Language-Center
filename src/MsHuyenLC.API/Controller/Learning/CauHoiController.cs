using MsHuyenLC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Application.Interfaces.System;

namespace MsHuyenLC.API.Controller.Learning;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,giaovu")]
public class CauHoiController : BaseController<CauHoi>
{
    private readonly ISystemLoggerService _systemLoggerService;
    public CauHoiController(
        IGenericService<CauHoi> service,
        ISystemLoggerService systemLoggerService
        ) : base(service)
    {
        _systemLoggerService = systemLoggerService;
    }

    protected override Func<IQueryable<CauHoi>, IOrderedQueryable<CauHoi>>? BuildOrderBy(string sortBy, string? sortOrder)
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
            return BadRequest(ModelState);
        }

        var createdCauHois = new List<CauHoi>();
        foreach (var request in requests)
        {
            var cauHoi = new CauHoi
            {
                NoiDung = request.NoiDung,
                LoaiCauHoi = request.LoaiCauHoi,
                KyNang = request.KyNang,
                UrlHinh = request.UrlHinh,
                UrlAmThanh = request.UrlAmThanh,
                DapAnDung = request.DapAnDung,
                GiaiThich = request.GiaiThich,
                CapDo = request.CapDo,
                DoKho = request.DoKho,
                DocHieuId = request.DocHieuId
            };
            var result = await _service.AddAsync(cauHoi);
            await _systemLoggerService.LogCreateAsync(GetCurrentUserId(), cauHoi, GetClientIpAddress());
            if (result == null)
            {
                return BadRequest("Không thể tạo câu hỏi.");
            }
            createdCauHois.Add(cauHoi);
        }
        
        return Ok(createdCauHois);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CauHoiUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCauHoi = await _service.GetByIdAsync(id);
        if (existingCauHoi == null)
        {
            return NotFound("Câu hỏi không tồn tại.");
        }

        var oldData = new CauHoi
        {
            Id = existingCauHoi.Id,
            NoiDung = existingCauHoi.NoiDung,
            LoaiCauHoi = existingCauHoi.LoaiCauHoi,
            KyNang = existingCauHoi.KyNang,
            UrlHinh = existingCauHoi.UrlHinh,
            UrlAmThanh = existingCauHoi.UrlAmThanh,
            DapAnDung = existingCauHoi.DapAnDung,
            GiaiThich = existingCauHoi.GiaiThich,
            CapDo = existingCauHoi.CapDo,
            DoKho = existingCauHoi.DoKho,
            DocHieuId = existingCauHoi.DocHieuId
        };

        if (request.NoiDung != null)
            existingCauHoi.NoiDung = request.NoiDung;
        if (request.LoaiCauHoi.HasValue)
            existingCauHoi.LoaiCauHoi = request.LoaiCauHoi.Value;
        if (request.KyNang.HasValue)
            existingCauHoi.KyNang = request.KyNang.Value;
        if (request.UrlHinh != null)
            existingCauHoi.UrlHinh = request.UrlHinh;
        if (request.UrlAmThanh != null)
            existingCauHoi.UrlAmThanh = request.UrlAmThanh;
        if (request.DapAnDung != null)
            existingCauHoi.DapAnDung = request.DapAnDung;
        if (request.GiaiThich != null)
            existingCauHoi.GiaiThich = request.GiaiThich;
        if (request.CapDo.HasValue)
            existingCauHoi.CapDo = request.CapDo.Value;
        if (request.DoKho.HasValue)
            existingCauHoi.DoKho = request.DoKho.Value;
        if (request.DocHieuId.HasValue)
            existingCauHoi.DocHieuId = request.DocHieuId;

        await _service.UpdateAsync(existingCauHoi);
        await _systemLoggerService.LogUpdateAsync(GetCurrentUserId(), oldData, existingCauHoi, GetClientIpAddress());
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingCauHoi = await _service.GetByIdAsync(id);
        if (existingCauHoi == null)
        {
            return NotFound("Câu hỏi không tồn tại.");
        }

        await _service.DeleteAsync(existingCauHoi);
        await _systemLoggerService.LogDeleteAsync(GetCurrentUserId(), existingCauHoi, GetClientIpAddress());
        return Ok();
    }
}