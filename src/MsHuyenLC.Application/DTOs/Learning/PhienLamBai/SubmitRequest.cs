namespace MsHuyenLC.Application.DTOs.Learning.PhienLamBai;

public class SubmitRequest
{
    public string DeThiId { get; set; } = string.Empty;
    public int TongCauHoi { get; set; }
    public Dictionary<Guid, string> CacTraLoi { get; set; } = new Dictionary<Guid, string>();
    public long ThoiGianLamBai { get; set; }
    public bool TuDongCham { get; set; } = true;
}

