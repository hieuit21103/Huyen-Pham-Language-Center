using System.Text.Json;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Repositories;
using MsHuyenLC.Application.Interfaces.Services;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.System;

namespace MsHuyenLC.Application.Services.System;

public class SystemLoggerService : ISystemLoggerService
{
    private readonly IUnitOfWork _unitOfWork;

    public SystemLoggerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Read Operations

    public async Task<NhatKyHeThong?> GetByIdAsync(string id)
    {
        return await _unitOfWork.NhatKyHeThongs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<NhatKyHeThong>> GetAllAsync()
    {
        return await _unitOfWork.NhatKyHeThongs.GetAllAsync();
    }

    public async Task<IEnumerable<NhatKyHeThong>> GetByUserIdAsync(Guid userId)
    {
        return await _unitOfWork.NhatKyHeThongs.GetAllAsync(
            filter: log => log.TaiKhoanId == userId
        );
    }

    public async Task<IEnumerable<NhatKyHeThong>> GetByActionAsync(string action)
    {
        return await _unitOfWork.NhatKyHeThongs.GetAllAsync(
            filter: log => log.HanhDong.Contains(action)
        );
    }

    public async Task<IEnumerable<NhatKyHeThong>> GetByDateRangeAsync(DateOnly from, DateOnly to)
    {
        return await _unitOfWork.NhatKyHeThongs.GetAllAsync(
            filter: log => log.ThoiGian >= from && log.ThoiGian <= to
        );
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.NhatKyHeThongs.CountAsync();
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await _unitOfWork.NhatKyHeThongs.CountAsync(
            predicate: log => log.TaiKhoanId == userId
        );
    }

    public async Task<int> CountByActionAsync(string action)
    {
        return await _unitOfWork.NhatKyHeThongs.CountAsync(
            predicate: log => log.HanhDong.Contains(action)
        );
    }

    #endregion

    #region Write Operations - Logging

    public async Task LogAsync(
        Guid taiKhoanId,
        string hanhDong,
        string? chiTiet = null,
        string duLieuCu = "",
        string duLieuMoi = "",
        string ip = "")
    {
        try
        {
            var log = new NhatKyHeThong
            {
                Id = Guid.NewGuid(),
                TaiKhoanId = taiKhoanId,
                HanhDong = hanhDong,
                ChiTiet = chiTiet,
                DuLieuCu = duLieuCu ?? "",
                DuLieuMoi = duLieuMoi ?? "",
                IP = ip ?? "",
                ThoiGian = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _unitOfWork.NhatKyHeThongs.AddAsync(log);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERROR] Failed to write system log: {ex.Message}");
        }
    }

    public async Task LogCreateAsync<T>(Guid taiKhoanId, T entity, string ip = "") where T : class
    {
        var entityName = typeof(T).Name;
        var duLieuMoi = SerializeEntity(entity);

        await LogAsync(
            taiKhoanId,
            $"Tạo {entityName}",
            $"Tạo mới {entityName}",
            "",
            duLieuMoi,
            ip
        );
    }

    public async Task LogUpdateAsync<T>(Guid taiKhoanId, T oldEntity, T newEntity, string ip = "") where T : class
    {
        var entityName = typeof(T).Name;
        var duLieuCu = SerializeEntity(oldEntity);
        var duLieuMoi = SerializeEntity(newEntity);

        var changes = GetChanges(oldEntity, newEntity);
        var chiTiet = changes.Any()
            ? $"Cập nhật {entityName}: {string.Join(", ", changes)}"
            : $"Cập nhật {entityName}";

        await LogAsync(
            taiKhoanId,
            $"Sửa {entityName}",
            chiTiet,
            duLieuCu,
            duLieuMoi,
            ip
        );
    }

    public async Task LogDeleteAsync<T>(Guid taiKhoanId, T entity, string ip = "") where T : class
    {
        var entityName = typeof(T).Name;
        var duLieuCu = SerializeEntity(entity);

        await LogAsync(
            taiKhoanId,
            $"Xóa {entityName}",
            $"Xóa {entityName}",
            duLieuCu,
            "",
            ip
        );
    }

    public async Task LogLoginAsync(Guid taiKhoanId, string ip, bool success)
    {
        var hanhDong = success ? "Đăng nhập thành công" : "Đăng nhập thất bại";
        var chiTiet = success
            ? $"Người dùng đăng nhập thành công từ IP {ip}"
            : $"Đăng nhập thất bại từ IP {ip}";

        await LogAsync(
            taiKhoanId,
            hanhDong,
            chiTiet,
            "",
            "",
            ip
        );
    }

    public async Task LogLogoutAsync(Guid taiKhoanId, string ip)
    {
        await LogAsync(
            taiKhoanId,
            "Đăng xuất",
            $"Người dùng đăng xuất từ IP {ip}",
            "",
            "",
            ip
        );
    }

    public async Task SaveChangesAsync()
    {
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Private Helper Methods

    private string SerializeEntity<T>(T entity) where T : class
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = global::System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
            return JsonSerializer.Serialize(entity, options);
        }
        catch
        {
            return "{}";
        }
    }

    private List<string> GetChanges<T>(T oldEntity, T newEntity) where T : class
    {
        var changes = new List<string>();

        try
        {
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                    continue;

                var oldValue = prop.GetValue(oldEntity);
                var newValue = prop.GetValue(newEntity);

                if (!Equals(oldValue, newValue))
                {
                    changes.Add($"{prop.Name}: {oldValue} → {newValue}");
                }
            }
        }
        catch
        {
        }

        return changes;
    }
    #endregion
}

