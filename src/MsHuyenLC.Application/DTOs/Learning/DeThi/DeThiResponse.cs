namespace MsHuyenLC.Application.DTOs.Learning.DeThi;

public class DeThiResponse
{
    public Guid Id { get; set; }
    public string TenDe { get; set; } = null!;
    public int TongCauHoi { get; set; }
    public Guid KyThiId { get; set; }
    public string? TenKyThi { get; set; }
}
