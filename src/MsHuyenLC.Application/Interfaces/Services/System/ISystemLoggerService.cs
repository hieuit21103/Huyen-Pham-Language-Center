using MsHuyenLC.Domain.Entities.System;

namespace MsHuyenLC.Application.Interfaces.Services.System;

public interface ISystemLoggerService
{
    Task<NhatKyHeThong?> GetByIdAsync(string id);
    Task<IEnumerable<NhatKyHeThong>> GetAllAsync();
    Task<IEnumerable<NhatKyHeThong>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<NhatKyHeThong>> GetByActionAsync(string action);
    Task<IEnumerable<NhatKyHeThong>> GetByDateRangeAsync(DateOnly from, DateOnly to);
    Task<int> CountAsync();
    Task<int> CountByUserIdAsync(Guid userId);
    Task<int> CountByActionAsync(string action);
    Task LogAsync(
        Guid taiKhoanId,
        string hanhDong,
        string? chiTiet = null,
        string duLieuCu = "",
        string duLieuMoi = "",
        string ip = "");

    Task LogCreateAsync<T>(Guid taiKhoanId, T entity, string ip = "") where T : class;
    Task LogUpdateAsync<T>(Guid taiKhoanId, T oldEntity, T newEntity, string ip = "") where T : class;
    Task LogDeleteAsync<T>(Guid taiKhoanId, T entity, string ip = "") where T : class;
    Task LogLoginAsync(Guid taiKhoanId, string ip, bool success);
    Task LogLogoutAsync(Guid taiKhoanId, string ip);
    Task SaveChangesAsync();
}
