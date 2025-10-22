namespace MsHuyenLC.Application.DTOs.Learning.CauHoi;

public class DapAnRequest
{
    public string Nhan { get; set; } = string.Empty; // A, B, C, D
    public string NoiDung { get; set; } = string.Empty;
    public bool Dung { get; set; }
    public string? GiaiThich { get; set; }
}

