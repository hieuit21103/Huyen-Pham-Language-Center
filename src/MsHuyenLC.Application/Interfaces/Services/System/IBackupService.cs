using MsHuyenLC.Domain.Entities.System;

namespace MsHuyenLC.Application.Interfaces.Services.System;

public interface IBackupService
{
    Task<SaoLuuDuLieu?> GetByIdAsync(string id);
    Task<IEnumerable<SaoLuuDuLieu>> GetAllAsync();
    Task<IEnumerable<SaoLuuDuLieu>> GetByDateRangeAsync(DateOnly from, DateOnly to);
    Task<SaoLuuDuLieu?> GetLatestAsync();
    Task<int> CountAsync();
    Task<SaoLuuDuLieu> CreateBackupAsync(string duongDan);
    Task<bool> DeleteAsync(string id);
    Task<bool> RestoreBackupAsync(string id);
    Task<string> ExportDatabaseAsync();
    Task<bool> ImportDatabaseAsync(string filePath);
    Task<string> UploadBackupFileAsync(Stream fileStream, string fileName);
    Task<string> GetPresignedUrlAsync(string objectName);
    Task SaveChangesAsync();
}
