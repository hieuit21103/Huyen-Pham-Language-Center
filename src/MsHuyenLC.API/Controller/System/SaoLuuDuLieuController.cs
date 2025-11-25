using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsHuyenLC.Application.Interfaces.Services.System;

namespace MsHuyenLC.API.Controller.System;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class SaoLuuDuLieuController : BaseController
{
    private readonly IBackupService _backupService;

    public SaoLuuDuLieuController(
        ISystemLoggerService logService,
        IBackupService backupService) : base(logService)
    {
        _backupService = backupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var backups = await _backupService.GetAllAsync();
            
            var backupsWithUrls = new List<object>();
            foreach (var backup in backups)
            {
                var presignedUrl = await _backupService.GetPresignedUrlAsync(backup.DuongDan);
                backupsWithUrls.Add(new
                {
                    backup.Id,
                    backup.NgaySaoLuu,
                    DuongDan = presignedUrl,
                    ObjectName = backup.DuongDan
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Lấy danh sách sao lưu thành công",
                data = backupsWithUrls
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi lấy danh sách sao lưu: {ex.Message}"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var backup = await _backupService.GetByIdAsync(id);
            if (backup == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bản sao lưu"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Lấy thông tin sao lưu thành công",
                data = backup
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi lấy thông tin sao lưu: {ex.Message}"
            });
        }
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest()
    {
        try
        {
            var backup = await _backupService.GetLatestAsync();
            if (backup == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Chưa có bản sao lưu nào"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Lấy bản sao lưu mới nhất thành công",
                data = backup
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi lấy bản sao lưu mới nhất: {ex.Message}"
            });
        }
    }

    [HttpGet("by-date-range")]
    public async Task<IActionResult> GetByDateRange(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        try
        {
            var backups = await _backupService.GetByDateRangeAsync(from, to);
            return Ok(new
            {
                success = true,
                message = "Lấy danh sách sao lưu theo khoảng thời gian thành công",
                data = backups
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi lấy danh sách sao lưu: {ex.Message}"
            });
        }
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        try
        {
            var count = await _backupService.CountAsync();
            return Ok(new
            {
                success = true,
                message = "Đếm số lượng bản sao lưu thành công",
                data = count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi đếm số lượng bản sao lưu: {ex.Message}"
            });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBackup()
    {
        try
        {
            var minioPath = await _backupService.ExportDatabaseAsync();

            var backup = await _backupService.CreateBackupAsync(minioPath);
            await _backupService.SaveChangesAsync();

            await LogCreateAsync(backup);

            return Ok(new
            {
                success = true,
                message = "Tạo bản sao lưu thành công",
                data = backup
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi tạo bản sao lưu: {ex.Message}"
            });
        }
    }

    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreBackup(string id)
    {
        try
        {
            var backup = await _backupService.GetByIdAsync(id);
            if (backup == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bản sao lưu"
                });
            }

            var result = await _backupService.RestoreBackupAsync(id);
            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Khôi phục bản sao lưu thất bại"
                });
            }

            // Log action
            await _logService.LogAsync(
                GetCurrentUserId(),
                "Khôi phục dữ liệu",
                $"Khôi phục từ bản sao lưu {backup.NgaySaoLuu}",
                "",
                backup.DuongDan,
                GetClientIpAddress()
            );
            await _backupService.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Khôi phục bản sao lưu thành công"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi khôi phục bản sao lưu: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var backup = await _backupService.GetByIdAsync(id);
            if (backup == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bản sao lưu"
                });
            }

            var result = await _backupService.DeleteAsync(id);
            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Xóa bản sao lưu thất bại"
                });
            }

            await _backupService.SaveChangesAsync();

            // Log action
            await LogDeleteAsync(backup);

            return Ok(new
            {
                success = true,
                message = "Xóa bản sao lưu thành công"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi xóa bản sao lưu: {ex.Message}"
            });
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadBackupFile([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Vui lòng chọn tệp sao lưu để tải lên"
                });
            }
            
            await using var stream = file.OpenReadStream();
            var minioPath = await _backupService.UploadBackupFileAsync(stream, file.FileName);
            var backup = await _backupService.CreateBackupAsync(minioPath);
            await _backupService.SaveChangesAsync();

            await LogCreateAsync(backup);

            return Ok(new
            {
                success = true,
                message = "Tải lên tệp sao lưu thành công",
                data = backup
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Lỗi khi tải lên tệp sao lưu: {ex.Message}"
            });
        }
    }
}
