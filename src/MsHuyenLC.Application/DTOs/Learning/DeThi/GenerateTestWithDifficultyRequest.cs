using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DeThi;

public class GenerateTestWithDifficultyRequest
{
    public string TenDe { get; set; } = null!;
    public int SoCauDe { get; set; }
    public int SoCauTrungBinh { get; set; }
    public int SoCauKho { get; set; }
    public int ThoiGianLamBai { get; set; }
    public LoaiDeThi LoaiDeThi { get; set; } = LoaiDeThi.LuyenTap;
    public LoaiCauHoi LoaiCauHoi { get; set; }
    public KyNang KyNang { get; set; }
    public CapDo CapDo { get; set; }
    public Guid? KyThiId { get; set; }
    public Guid? NguoiTaoId { get; set; }
}
