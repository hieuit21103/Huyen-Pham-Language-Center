namespace MsHuyenLC.Application.DTOs.Learning.CauTraLoi;

public class DapAnResponse
{
    public string? Nhan { get; set; }
    public string? NoiDung { get; set; }
    public bool Dung { get; set; }
    public string? GiaiThich { get; set; }
}

public class CauTraLoiResponse
{
    public Guid Id { get; set; }
    public string? NoiDungCauHoi { get; set; }
    public string? NoiDung { get; set; }
    public bool? Dung { get; set; }
    public string? DapAnDung { get; set; }
    public string? GiaiThich { get; set; }
    public List<DapAnResponse>? CacDapAn { get; set; }
}