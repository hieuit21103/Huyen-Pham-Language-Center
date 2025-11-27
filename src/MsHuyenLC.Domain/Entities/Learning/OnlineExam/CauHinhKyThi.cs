namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class CauHinhKyThi
{
    public Guid Id { get; set; }
    public CapDo CapDo { get; set; }
    public DoKho DoKho { get; set; }
    public KyNang KyNang { get; set; }
    public CheDoCauHoi CheDoCauHoi { get; set; }
    public int SoCauHoi { get; set; }
    public Guid KyThiId { get; set; }
    public KyThi KyThi { get; set; } = null!;
}