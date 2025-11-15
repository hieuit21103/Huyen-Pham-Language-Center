using MsHuyenLC.Application.Interfaces.Services.Excel;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using ClosedXML.Excel;

namespace MsHuyenLC.Infrastructure.Services.Excel;

public class ExcelService : IExcelService
{

    public async Task<IEnumerable<CauHoiImportRequest>> ImportQuestionsFromExcelAsync(Stream excelStream)
    {
        var importRequests = new List<CauHoiImportRequest>();

        using (var workbook = new XLWorkbook(excelStream))
        {
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                try
                {
                    var noiDungCauHoi = row.Cell(1).GetString().Trim();
                    
                    if (string.IsNullOrWhiteSpace(noiDungCauHoi))
                        continue;

                    var request = new CauHoiImportRequest
                    {
                        NoiDungCauHoi = noiDungCauHoi,
                        LoaiCauHoi = NormalizeEnumValue(row.Cell(2).GetString()),
                        KyNang = NormalizeEnumValue(row.Cell(3).GetString()),
                        DoKho = NormalizeEnumValue(row.Cell(4).GetString()),
                        CapDo = row.Cell(5).GetString().Trim().ToUpper(),
                        UrlAmThanh = GetNullableString(row.Cell(6)),
                        UrlHinhAnh = GetNullableString(row.Cell(7)),
                        LoiThoai = GetNullableString(row.Cell(8)),
                        DapAnA = row.Cell(9).GetString().Trim(),
                        DapAnB = GetNullableString(row.Cell(10)),
                        DapAnC = GetNullableString(row.Cell(11)),
                        DapAnD = GetNullableString(row.Cell(12)),
                        DapAnDung = row.Cell(13).GetString().Trim().ToUpper(),
                        GiaiThich = GetNullableString(row.Cell(14))
                    };

                    if (!string.IsNullOrWhiteSpace(request.DapAnA) && 
                        !string.IsNullOrWhiteSpace(request.DapAnDung))
                    {
                        importRequests.Add(request);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        return await Task.FromResult(importRequests.AsEnumerable());
    }

    private static string NormalizeEnumValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim()
            .ToLower()
            .Replace(" ", "")
            .Replace("ă", "a")
            .Replace("â", "a")
            .Replace("á", "a")
            .Replace("à", "a")
            .Replace("ả", "a")
            .Replace("ã", "a")
            .Replace("ạ", "a")
            .Replace("ấ", "a")
            .Replace("ầ", "a")
            .Replace("ẩ", "a")
            .Replace("ẫ", "a")
            .Replace("ậ", "a")
            .Replace("ắ", "a")
            .Replace("ằ", "a")
            .Replace("ẳ", "a")
            .Replace("ẵ", "a")
            .Replace("ặ", "a")
            .Replace("đ", "d")
            .Replace("é", "e")
            .Replace("è", "e")
            .Replace("ẻ", "e")
            .Replace("ẽ", "e")
            .Replace("ẹ", "e")
            .Replace("ê", "e")
            .Replace("ế", "e")
            .Replace("ề", "e")
            .Replace("ể", "e")
            .Replace("ễ", "e")
            .Replace("ệ", "e")
            .Replace("í", "i")
            .Replace("ì", "i")
            .Replace("ỉ", "i")
            .Replace("ĩ", "i")
            .Replace("ị", "i")
            .Replace("ó", "o")
            .Replace("ò", "o")
            .Replace("ỏ", "o")
            .Replace("õ", "o")
            .Replace("ọ", "o")
            .Replace("ô", "o")
            .Replace("ố", "o")
            .Replace("ồ", "o")
            .Replace("ổ", "o")
            .Replace("ỗ", "o")
            .Replace("ộ", "o")
            .Replace("ơ", "o")
            .Replace("ớ", "o")
            .Replace("ờ", "o")
            .Replace("ở", "o")
            .Replace("ỡ", "o")
            .Replace("ợ", "o")
            .Replace("ú", "u")
            .Replace("ù", "u")
            .Replace("ủ", "u")
            .Replace("ũ", "u")
            .Replace("ụ", "u")
            .Replace("ư", "u")
            .Replace("ứ", "u")
            .Replace("ừ", "u")
            .Replace("ử", "u")
            .Replace("ữ", "u")
            .Replace("ự", "u")
            .Replace("ý", "y")
            .Replace("ỳ", "y")
            .Replace("ỷ", "y")
            .Replace("ỹ", "y")
            .Replace("ỵ", "y");

        return normalized switch
        {
            "de" or "dễ" or "easy" => "de",
            "trungbinh" or "trung binh" or "tb" or "medium" => "trungbinh",
            "kho" or "khó" or "hard" or "difficult" => "kho",
            "tracnghiem" or "trac nghiem" or "multiple choice" => "TracNghiem",
            "tuluan" or "tu luan" or "essay" => "TuLuan",
            "nghe" or "listening" or "listen" => "Nghe",
            "doc" or "đọc" or "reading" or "read" => "Doc",
            "viet" or "viết" or "writing" or "write" => "Viet",
            _ => normalized
        };
    }

    private static string? GetNullableString(IXLCell cell)
    {
        var value = cell.GetString().Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public async Task DownloadQuestionsTemplateAsync(Stream outputStream)
    {
        var templateStream = Templates.ExcelTemplateHelper.GetQuestionsTemplateFile();
        templateStream.Position = 0;
        await templateStream.CopyToAsync(outputStream);
    }
}