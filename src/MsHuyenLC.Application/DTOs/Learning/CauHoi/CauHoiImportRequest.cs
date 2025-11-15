namespace MsHuyenLC.Application.DTOs.Learning.CauHoi;

public class CauHoiImportRequest
{
    public string NoiDungCauHoi { get; set; } = string.Empty;
    public string LoaiCauHoi { get; set; } = "TracNghiem";
    public string KyNang { get; set; } = "Doc";
    public string DoKho { get; set; } = "trungbinh";
    public string CapDo { get; set; } = "A1";
    public string? UrlAmThanh { get; set; }
    public string? UrlHinhAnh { get; set; }
    public string? LoiThoai { get; set; }
    
    // Đáp án
    public string DapAnA { get; set; } = string.Empty;
    public string? DapAnB { get; set; }
    public string? DapAnC { get; set; }
    public string? DapAnD { get; set; }
    
    // Đáp án đúng (A, B, C, hoặc D)
    public string DapAnDung { get; set; } = string.Empty;
    
    // Giải thích chung cho câu hỏi
    public string? GiaiThich { get; set; }
}

public class CauHoiImportResult
{
    public int TongSo { get; set; }
    public int ThanhCong { get; set; }
    public int ThatBai { get; set; }
    public List<string> LoiChiTiet { get; set; } = new();
}
