namespace MsHuyenLC.Domain.Entities.Learning;

public class DocHieu
{
    public Guid Id { get; set; }
    public string NoiDung { get; set; } = null!;
    public CapDo CapDo { get; set; } = CapDo.A1;
    public DoKho DoKho { get; set; } = DoKho.de;
    public ICollection<CauHoi> CauHois { get; set; } = new List<CauHoi>();
}