using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.API.Controller.Learning;

/// <summary>
/// Controller quản lý kỳ thi
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class KyThiController : BaseController<KyThi>
{
    public KyThiController(IGenericService<KyThi> service) : base(service)
    {
    }

    /// <summary>
    /// Tạo kỳ thi mới
    /// </summary>
    /// <param name="request">Thông tin kỳ thi</param>
    /// <returns>Kỳ thi đã tạo</returns>
    /// <response code="200">Tạo kỳ thi thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] KyThiRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kyThi = new KyThi
            {
                TenKyThi = request.TenKyThi,
                NgayThi = request.NgayThi,
                ThoiLuong = request.ThoiLuong,
                LopHocId = request.LopHocId,
                HinhThuc = request.HinhThuc,
                TrangThai = TrangThaiKyThi.sapdienra
            };

            var createdKyThi = await _service.AddAsync(kyThi);

            return Ok(new
            {
                success = true,
                message = "Tạo kỳ thi thành công",
                data = new
                {
                    id = createdKyThi.Id,
                    tenKyThi = createdKyThi.TenKyThi,
                    ngayThi = createdKyThi.NgayThi,
                    thoiLuong = createdKyThi.ThoiLuong,
                    lopHocId = createdKyThi.LopHocId,
                    hinhThuc = createdKyThi.HinhThuc,
                    trangThai = createdKyThi.TrangThai
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi tạo kỳ thi",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật thông tin kỳ thi
    /// </summary>
    /// <param name="id">ID kỳ thi</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Kỳ thi đã cập nhật</returns>
    /// <response code="200">Cập nhật thành công</response>
    /// <response code="404">Không tìm thấy kỳ thi</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="500">Lỗi server</response>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] KyThiUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingKyThi = await _service.GetByIdAsync(id);
            if (existingKyThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kỳ thi"
                });
            }

            existingKyThi.TenKyThi = request.TenKyThi;
            existingKyThi.NgayThi = request.NgayThi;
            existingKyThi.ThoiLuong = request.ThoiLuong;
            existingKyThi.LopHocId = request.LopHocId;
            existingKyThi.HinhThuc = request.HinhThuc;
            existingKyThi.TrangThai = request.TrangThai;

            await _service.UpdateAsync(existingKyThi);

            return Ok(new
            {
                success = true,
                message = "Cập nhật kỳ thi thành công",
                data = new
                {
                    id = existingKyThi.Id,
                    tenKyThi = existingKyThi.TenKyThi,
                    ngayThi = existingKyThi.NgayThi,
                    thoiLuong = existingKyThi.ThoiLuong,
                    lopHocId = existingKyThi.LopHocId,
                    hinhThuc = existingKyThi.HinhThuc,
                    trangThai = existingKyThi.TrangThai
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi cập nhật kỳ thi",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Xóa kỳ thi
    /// </summary>
    /// <param name="id">ID kỳ thi</param>
    /// <returns>Kết quả xóa</returns>
    /// <response code="200">Xóa thành công</response>
    /// <response code="404">Không tìm thấy kỳ thi</response>
    /// <response code="500">Lỗi server</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var kyThi = await _service.GetByIdAsync(id);
            if (kyThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kỳ thi"
                });
            }

            await _service.DeleteAsync(kyThi);

            return Ok(new
            {
                success = true,
                message = "Xóa kỳ thi thành công"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi xóa kỳ thi",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật trạng thái kỳ thi
    /// </summary>
    /// <param name="id">ID kỳ thi</param>
    /// <param name="trangThai">Trạng thái mới (0=SapDienRa, 1=DangDienRa, 2=KetThuc)</param>
    /// <returns>Kỳ thi với trạng thái mới</returns>
    /// <response code="200">Cập nhật trạng thái thành công</response>
    /// <response code="404">Không tìm thấy kỳ thi</response>
    /// <response code="500">Lỗi server</response>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateStatus(string id, [FromBody] TrangThaiKyThi trangThai)
    {
        try
        {
            var kyThi = await _service.GetByIdAsync(id);
            if (kyThi == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kỳ thi"
                });
            }

            kyThi.TrangThai = trangThai;
            await _service.UpdateAsync(kyThi);

            return Ok(new
            {
                success = true,
                message = "Cập nhật trạng thái kỳ thi thành công",
                data = new
                {
                    id = kyThi.Id,
                    trangThai = kyThi.TrangThai
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi cập nhật trạng thái",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy danh sách kỳ thi theo lớp học
    /// </summary>
    /// <param name="lopHocId">ID lớp học</param>
    /// <returns>Danh sách kỳ thi của lớp</returns>
    /// <response code="200">Lấy danh sách thành công</response>
    /// <response code="500">Lỗi server</response>
    [HttpGet("lop/{lopHocId}")]
    public async Task<ActionResult> GetByLopHoc(string lopHocId)
    {
        try
        {
            var kyThis = await _service.GetAllAsync(
                PageNumber: 1,
                PageSize: 100,
                Filter: kt => kt.LopHocId.ToString() == lopHocId
            );

            return Ok(new
            {
                success = true,
                data = kyThis.Select(kt => new
                {
                    id = kt.Id,
                    tenKyThi = kt.TenKyThi,
                    ngayThi = kt.NgayThi,
                    thoiLuong = kt.ThoiLuong,
                    hinhThuc = kt.HinhThuc,
                    trangThai = kt.TrangThai
                }),
                total = kyThis.Count()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi lấy danh sách kỳ thi",
                error = ex.Message
            });
        }
    }
}
