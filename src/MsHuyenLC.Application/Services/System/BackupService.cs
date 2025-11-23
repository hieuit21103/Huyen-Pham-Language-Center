using System.Diagnostics;
using Minio;
using Minio.DataModel.Args;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.System;
using MsHuyenLC.Domain.Entities.System;

namespace MsHuyenLC.Application.Services.System;

public class BackupService : IBackupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _connectionString;
    private readonly IMinioClient _minioClient;
    private readonly string _minioBucket;
    private readonly string _minioDomain;
    private readonly string _backupDirectory;

    public BackupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
            ?? "localhost";
        
        _minioBucket = "backups";
        _backupDirectory = Path.Combine(Path.GetTempPath(), "backups");
        Directory.CreateDirectory(_backupDirectory);

        var minioEndpoint = Environment.GetEnvironmentVariable("Minio__Endpoint") ?? "host.docker.internal:9000";
        var minioAccessKey = Environment.GetEnvironmentVariable("Minio__AccessKey") ?? "admin123";
        var minioSecretKey = Environment.GetEnvironmentVariable("Minio__SecretKey") ?? "admin123";
        _minioDomain = Environment.GetEnvironmentVariable("Minio__Domain") ?? $"http://{minioEndpoint}";

        _minioClient = new MinioClient()
            .WithEndpoint(minioEndpoint)
            .WithCredentials(minioAccessKey, minioSecretKey)
            .Build();
    }

    public async Task<SaoLuuDuLieu?> GetByIdAsync(string id)
    {
        return await _unitOfWork.SaoLuuDuLieus.GetByIdAsync(id);
    }

    public async Task<IEnumerable<SaoLuuDuLieu>> GetAllAsync()
    {
        var backups = await _unitOfWork.SaoLuuDuLieus.GetAllAsync();
        return backups.OrderByDescending(s => s.NgaySaoLuu);
    }

    public async Task<IEnumerable<SaoLuuDuLieu>> GetByDateRangeAsync(DateOnly from, DateOnly to)
    {
        var backups = await _unitOfWork.SaoLuuDuLieus.GetAllAsync(
            filter: backup => backup.NgaySaoLuu >= from && backup.NgaySaoLuu <= to
        );
        return backups.OrderByDescending(s => s.NgaySaoLuu);
    }

    public async Task<SaoLuuDuLieu?> GetLatestAsync()
    {
        var backups = await _unitOfWork.SaoLuuDuLieus.GetAllAsync();
        return backups.OrderByDescending(s => s.NgaySaoLuu).FirstOrDefault();
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.SaoLuuDuLieus.CountAsync();
    }

    public async Task<SaoLuuDuLieu> CreateBackupAsync(string duongDan)
    {
        var backup = new SaoLuuDuLieu
        {
            Id = Guid.NewGuid(),
            NgaySaoLuu = DateOnly.FromDateTime(DateTime.UtcNow),
            DuongDan = duongDan
        };

        await _unitOfWork.SaoLuuDuLieus.AddAsync(backup);
        return backup;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var backup = await _unitOfWork.SaoLuuDuLieus.GetByIdAsync(id);
        if (backup == null)
            return false;

        if (backup.DuongDan.StartsWith("http") && backup.DuongDan.Contains(_minioBucket))
        {
            try
            {
                var uri = new Uri(backup.DuongDan);
                var objectName = uri.AbsolutePath.TrimStart('/');
                if (objectName.StartsWith($"{_minioBucket}/"))
                {
                    objectName = objectName.Substring($"{_minioBucket}/".Length);
                }

                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(_minioBucket)
                    .WithObject(objectName));
            }
            catch
            {
            }
        }
        else if (File.Exists(backup.DuongDan))
        {
            try
            {
                File.Delete(backup.DuongDan);
            }
            catch
            {
            }
        }

        await _unitOfWork.SaoLuuDuLieus.DeleteAsync(backup);
        return true;
    }

    public async Task<bool> RestoreBackupAsync(string id)
    {
        var backup = await _unitOfWork.SaoLuuDuLieus.GetByIdAsync(id);
        if (backup == null)
            return false;

        try
        {
            string localFilePath;
            bool needsCleanup = false;

            if (backup.DuongDan.StartsWith("http") && backup.DuongDan.Contains(_minioBucket))
            {
                var uri = new Uri(backup.DuongDan);
                var objectName = uri.AbsolutePath.TrimStart('/');
                if (objectName.StartsWith($"{_minioBucket}/"))
                {
                    objectName = objectName.Substring($"{_minioBucket}/".Length);
                }

                localFilePath = Path.Combine(_backupDirectory, $"restore_{Guid.NewGuid()}.sql");
                await _minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(_minioBucket)
                    .WithObject(objectName)
                    .WithFile(localFilePath));
                needsCleanup = true;
            }
            else
            {
                localFilePath = backup.DuongDan;
            }

            if (!File.Exists(localFilePath))
                return false;

            var connectionParams = ParseConnectionString(_connectionString);
            
            var processInfo = new ProcessStartInfo
            {
                FileName = "psql",
                Arguments = $"-h {connectionParams.Host} -p {connectionParams.Port} -U {connectionParams.Username} -d {connectionParams.Database} -f \"{localFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            processInfo.EnvironmentVariables["PGPASSWORD"] = connectionParams.Password;

            using var process = Process.Start(processInfo);
            if (process == null)
                return false;

            await process.WaitForExitAsync();
            
            if (needsCleanup && File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
            }

            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> ExportDatabaseAsync()
    {
        try
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = $"backup_{timestamp}.sql";
            var localFilePath = Path.Combine(_backupDirectory, fileName);

            var connectionParams = ParseConnectionString(_connectionString);
            
            var processInfo = new ProcessStartInfo
            {
                FileName = "pg_dump",
                Arguments = $"-h {connectionParams.Host} -p {connectionParams.Port} -U {connectionParams.Username} -d {connectionParams.Database} -F p --clean --if-exists -f \"{localFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            processInfo.EnvironmentVariables["PGPASSWORD"] = connectionParams.Password;

            using var process = Process.Start(processInfo);
            if (process == null)
                throw new Exception("Failed to start pg_dump process");

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"pg_dump failed: {error}");
            }

            bool bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_minioBucket));
            
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_minioBucket));
            }

            var objectName = fileName;
            await using var fileStream = File.OpenRead(localFilePath);
            
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_minioBucket)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType("application/sql"));

            File.Delete(localFilePath);

            var minioUrl = $"{_minioDomain}/{_minioBucket}/{objectName}";
            return minioUrl;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database export failed: {ex.Message}", ex);
        }
    }

    public async Task<bool> ImportDatabaseAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        try
        {
            var connectionParams = ParseConnectionString(_connectionString);
            
            var processInfo = new ProcessStartInfo
            {
                FileName = "psql",
                Arguments = $"-h {connectionParams.Host} -p {connectionParams.Port} -U {connectionParams.Username} -d {connectionParams.Database} -f \"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            processInfo.EnvironmentVariables["PGPASSWORD"] = connectionParams.Password;

            using var process = Process.Start(processInfo);
            if (process == null)
                return false;

            await process.WaitForExitAsync();
            
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private (string Host, string Port, string Database, string Username, string Password) ParseConnectionString(string connectionString)
    {
        var parts = connectionString.Split(';');
        var dict = new Dictionary<string, string>();
        
        foreach (var part in parts)
        {
            var keyValue = part.Split('=', 2);
            if (keyValue.Length == 2)
            {
                dict[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }

        return (
            Host: dict.GetValueOrDefault("Host", "localhost"),
            Port: dict.GetValueOrDefault("Port", "5432"),
            Database: dict.GetValueOrDefault("Database", ""),
            Username: dict.GetValueOrDefault("Username", ""),
            Password: dict.GetValueOrDefault("Password", "")
        );
    }

    public async Task SaveChangesAsync()
    {
        await _unitOfWork.SaveChangesAsync();
    }
}
