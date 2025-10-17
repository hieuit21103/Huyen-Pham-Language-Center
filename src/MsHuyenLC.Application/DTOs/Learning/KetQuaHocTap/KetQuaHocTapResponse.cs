namespace MsHuyenLC.Application.DTOs.Learning.KetQuaHocTap;

public class KetQuaHocTapResponse
{
    public Guid Id { get; set; }
    public Guid HocVienId { get; set; }
    public string? TenHocVien { get; set; }
    public Guid KyThiId { get; set; }
    public string? TenKyThi { get; set; }
    public decimal DiemSo { get; set; }
    public DateTime NgayThi { get; set; }
}
