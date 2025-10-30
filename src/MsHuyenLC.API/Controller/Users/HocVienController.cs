using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.System;
using Microsoft.AspNetCore.Authorization;
using MsHuyenLC.Application.DTOs.Users.HocVien;
using System.Security.Claims;

namespace MsHuyenLC.API.Controller.Users;

[Route("api/[controller]")]
[ApiController]
public class HocVienController : BaseController<HocVien>
{
    

    public HocVienController(IGenericService<HocVien> service, ISystemLoggerService logService) 
        : base(service, logService)
    {
    }

    protected override Func<IQueryable<HocVien>, IOrderedQueryable<HocVien>>? BuildOrderBy(string sortBy, string? sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "hoten" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.HoTen))
                : (q => q.OrderBy(k => k.HoTen)),
            "ngaysinh" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.NgaySinh))
                : (q => q.OrderBy(k => k.NgaySinh)),
            "gioitinh" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.GioiTinh))
                : (q => q.OrderBy(k => k.GioiTinh)),
            "diachi" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.DiaChi))
                : (q => q.OrderBy(k => k.DiaChi)),
            "ngaydangky" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.NgayDangKy))
                : (q => q.OrderBy(k => k.NgayDangKy)),
            "trangthai" => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.TrangThai))
                : (q => q.OrderBy(k => k.TrangThai)),
            _ => sortOrder?.ToLower() == "desc"
                ? (q => q.OrderByDescending(k => k.Id))
                : (q => q.OrderBy(k => k.Id)),
        };
    }

    [Authorize]
    [HttpGet("taikhoan/{id}")]
    public async Task<IActionResult> GetByAccountId(string id)
    {
        var result = await _service.GetAllAsync(
            PageNumber: 1,
            PageSize: 1,
            Filter: hv => hv.TaiKhoanId.ToString() == id
        );

        var hocVien = result.FirstOrDefault();
        if (hocVien == null) return NotFound();

        return Ok(hocVien);
    }

    [Authorize(Roles = "admin,giaovu,giaovien,hocvien")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] HocVienUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var hocVien = await _service.GetByIdAsync(id);
        if (hocVien == null) return NotFound();

        if(hocVien.TaiKhoanId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value &&
           !User.IsInRole("admin") && !User.IsInRole("giaovu"))
        {
            return Forbid();
        }

        var existingStudent = await _service.GetByIdAsync(id);
        if (existingStudent == null) return NotFound();

        existingStudent.HoTen = request.HoTen;
        existingStudent.NgaySinh = request.NgaySinh;
        existingStudent.GioiTinh = request.GioiTinh;
        existingStudent.DiaChi = request.DiaChi;
        existingStudent.TrangThai = request.TrangThai;

        await _service.UpdateAsync(existingStudent);

        return Ok(new { message = "Cập nhật học viên thành công." });
    }

    [Authorize(Roles = "admin,giaovu")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        await _service.DeleteAsync(entity);

        return Ok(new { message = "Xóa học viên thành công." });
    }
}
