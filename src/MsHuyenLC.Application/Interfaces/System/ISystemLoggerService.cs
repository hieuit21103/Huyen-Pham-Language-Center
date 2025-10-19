namespace MsHuyenLC.Application.Interfaces.System;

public interface ISystemLoggerService
{
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
