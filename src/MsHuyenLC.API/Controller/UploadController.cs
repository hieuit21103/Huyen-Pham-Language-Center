using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Minio;
using Minio.DataModel.Args;
using StackExchange.Redis;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IMinioClient _minio;
    private readonly string endpoint;
    private readonly string accessKey;
    private readonly string secretKey;
    private readonly string cdnDomain;

    public UploadController()
    {
        endpoint = Environment.GetEnvironmentVariable("Minio__Endpoint") ?? "host.docker.internal:9000";
        accessKey = Environment.GetEnvironmentVariable("Minio__AccessKey") ?? "admin123";
        secretKey = Environment.GetEnvironmentVariable("Minio__SecretKey") ?? "admin123";
        cdnDomain = Environment.GetEnvironmentVariable("Minio__Domain") ?? endpoint;

        _minio = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
    }

    [HttpPost("image")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        string _bucket = "images";
        string _allowedExtensions = ".jpg,.jpeg,.png,.gif,.bmp,.svg";
        long fileSizeLimit = 5 * 1024 * 1024; // 5 MB

        if (file == null || file.Length == 0) return BadRequest(new 
        { 
            success = false, 
            message = "Không có tệp được tải lên" 
        });

        if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            return BadRequest(
                new 
                { 
                    success = false, 
                    message = "Loại tệp không hợp lệ. Chỉ cho phép các tệp hình ảnh" 
                }
            );
        }

        if (file.Length > fileSizeLimit)
        {
            return BadRequest(
                new 
                { 
                    success = false, 
                    message = "Kích thước tệp vượt quá giới hạn 5 MB" 
                }
            );
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        using var stream = file.OpenReadStream();

        bool found = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
        if (!found)
            await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));

        await _minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));

        var url = $"{cdnDomain}/{_bucket}/{fileName}";
        return Ok(new 
        { 
            success = true, 
            message = "Tải lên hình ảnh thành công",
            data = new { url } 
        });
    }

    [HttpPost("audio")]
    [Authorize(Roles = "admin,giaovu")]
    public async Task<IActionResult> UploadAudio(IFormFile file)
    {
        string _bucket = "audio";
        string _allowedExtensions = ".mp3,.wav,.aac,.flac,.ogg";
        long fileSizeLimit = 10 * 1024 * 1024; // 10 MB

        if (file == null || file.Length == 0) return BadRequest(new 
        { 
            success = false, 
            message = "Không có tệp được tải lên" 
        });

        if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            return BadRequest(
                new 
                { 
                    success = false, 
                    message = "Tệp không hợp lệ. Chỉ cho phép các tệp âm thanh" 
                }
            );
        }

        if (file.Length > fileSizeLimit)
        {
            return BadRequest(
                new 
                { 
                    success = false, 
                    message = "Kích thước tệp vượt quá giới hạn 10 MB" 
                }
            );
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        using var stream = file.OpenReadStream();

        bool found = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
        if (!found)
            await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));

        await _minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));

        var url = $"{cdnDomain}/{_bucket}/{fileName}";
        return Ok(new 
        { 
            success = true, 
            message = "Tải lên âm thanh thành công",
            url
        });
    }

    [HttpPost("avatar")]
    [Authorize(Roles = "admin,giaovu,hocvien")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        string _bucket = "avatars";
        string _allowedExtensions = ".jpg,.jpeg,.png,.gif,.bmp,.svg";
        long fileSizeLimit = 2 * 1024 * 1024; // 2 MB

        if (file == null || file.Length == 0) return BadRequest(new 
        { 
            success = false, 
            message = "Không có tệp được tải lên" 
        });

        if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            return BadRequest(
                new 
                { 
                    success = false, 
                    message = "Loại tệp không hợp lệ. Chỉ cho phép các tệp hình ảnh" 
                }
            );
        }

        if (file.Length > fileSizeLimit)
        {
            return BadRequest(
                new 
                { 
                    success = false, 
                    message = "Kích thước tệp vượt quá giới hạn 2 MB" 
                }
            );
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        using var stream = file.OpenReadStream();

        bool found = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
        if (!found)
            await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));

        await _minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));

        var url = $"{cdnDomain}/{_bucket}/{fileName}";
        return Ok(new 
        { 
            success = true, 
            message = "Tải lên avatar thành công",
            url
        });
    }

    [HttpDelete("delete/{bucket}/{objectName}")]
    public async Task<IActionResult> DeleteObject(string bucket, string objectName)
    {
        try
        {
            await _minio.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName));
            return Ok(new
            {
                success = true,
                message = "Xóa tệp thành công"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new 
                { 
                    success = false, 
                    message = $"Lỗi: {ex.Message}" 
                });
        }
    }
}
