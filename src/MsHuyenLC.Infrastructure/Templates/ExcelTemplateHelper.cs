using ClosedXML.Excel;

namespace MsHuyenLC.Infrastructure.Templates;

public static class ExcelTemplateHelper
{
    private static readonly string TemplateBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Excel");
    
    public static MemoryStream GetExcelFile(string templateFileName)
    {
        var templatePath = Path.Combine(TemplateBasePath, templateFileName);
        
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        var memoryStream = new MemoryStream();
        
        using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            fileStream.CopyTo(memoryStream);
        }
        
        memoryStream.Position = 0;
        return memoryStream;
    }
    
    public static MemoryStream GetExcelFileWithReplacements(string templateFileName, Dictionary<string, string> replacements)
    {
        var templatePath = Path.Combine(TemplateBasePath, templateFileName);
        
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        var memoryStream = new MemoryStream();
        
        using (var workbook = new XLWorkbook(templatePath))
        {
            // Duyệt qua tất cả các worksheet
            foreach (var worksheet in workbook.Worksheets)
            {
                // Duyệt qua tất cả các cell có giá trị
                foreach (var cell in worksheet.CellsUsed())
                {
                    if (cell.Value.IsText)
                    {
                        var cellValue = cell.Value.GetText();
                        
                        // Thay thế tất cả placeholder {{Key}}
                        foreach (var replacement in replacements)
                        {
                            cellValue = cellValue.Replace($"{replacement.Key}", replacement.Value);
                        }
                        
                        cell.Value = cellValue;
                    }
                }
            }
            
            workbook.SaveAs(memoryStream);
        }
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public static MemoryStream CreateExcelFromData(List<Dictionary<string, object>> data, string sheetName = "Sheet1")
    {
        var memoryStream = new MemoryStream();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(sheetName);

            if (data.Count > 0)
            {
                // Tạo header từ keys của dictionary đầu tiên
                var headers = data[0].Keys.ToList();
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                }

                // Điền dữ liệu
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < headers.Count; col++)
                    {
                        var value = data[row][headers[col]];
                        worksheet.Cell(row + 2, col + 1).Value = XLCellValue.FromObject(value);
                    }
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();
            }

            workbook.SaveAs(memoryStream);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
    
    public static MemoryStream GetQuestionsTemplateFile()
    {
        var templateFileName = "QuestionsTemplate.xlsx";
        return GetExcelFile(templateFileName);
    }
}
