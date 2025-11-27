using MsHuyenLC.Application.DTOs.Learning.CauHoi;

namespace MsHuyenLC.Application.Interfaces.Services.Excel;

public interface IExcelService
{
    Task<IEnumerable<CauHoiImportRequest>> ImportQuestionsFromExcelAsync(Stream excelStream);
    Task DownloadQuestionsTemplateAsync(Stream outputStream);
}