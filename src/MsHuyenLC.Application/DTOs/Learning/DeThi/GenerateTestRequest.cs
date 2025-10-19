using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DeThi;

public class GenerateTestRequest
{
    public string TenDe { get; set; } = null!;
    public int SoCauHoi { get; set; }
    public int ThoiGianLamBai { get; set; }
    public LoaiDeThi LoaiDeThi { get; set; } = LoaiDeThi.LuyenTap;
    public LoaiCauHoi LoaiCauHoi { get; set; }
    public KyNang KyNang { get; set; }
    public CapDo CapDo { get; set; }
    public DoKho DoKho { get; set; }
    public Guid? KyThiId { get; set; }
}
