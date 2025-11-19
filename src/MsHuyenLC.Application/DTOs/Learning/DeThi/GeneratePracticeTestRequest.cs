using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DeThi;

public class GeneratePracticeTestRequest
{
    public CapDo CapDo { get; set; }
    public DoKho DoKho { get; set; }
    public KyNang KyNang { get; set; }
    public int SoCauHoi { get; set; }
    public int ThoiLuongPhut { get; set; }
    public CheDoCauHoi CheDoCauHoi { get; set; } = CheDoCauHoi.Don;
}
