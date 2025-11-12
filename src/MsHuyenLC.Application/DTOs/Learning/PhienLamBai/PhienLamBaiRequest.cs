namespace MsHuyenLC.Application.DTOs.Learning.PhienLamBai;

public class PhienLamBaiRequest
{
    public string DeThiId { get; set; } = string.Empty;
    public string HocVienId { get; set; } = string.Empty;
    public int? SoCauDung { get; set; }
    public decimal? Diem { get; set; }
    public TimeSpan ThoiGianLam { get; set; }
    public int TongCauHoi { get; set; }
}
