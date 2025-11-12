namespace MsHuyenLC.Application.Interfaces.Services;

public interface IUploadService
{
    Task<string> UploadFileAsync(byte[] fileData, string fileName, string contentType);
    Task<bool> DeleteFileAsync(string fileUrl);
}