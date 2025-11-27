using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.KyThi;

public class CauHinhKyThiResponse
{
    public CapDo  CapDo { get; set; }
    public DoKho DoKho { get; set; }
    public KyNang KyNang { get; set; }
    public CheDoCauHoi CheDoCauHoi { get; set; }
    public int SoCauHoi { get; set; }
    public Guid KyThiId { get; set; }
}