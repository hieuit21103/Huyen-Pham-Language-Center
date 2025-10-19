namespace MsHuyenLC.Domain.Entities.Learning;

public class CauHoi
{
    public Guid Id { get; set; }
    public string NoiDung { get; set; } = null!;
    public LoaiCauHoi LoaiCauHoi { get; set; } = LoaiCauHoi.TracNghiem;
    public KyNang KyNang { get; set; } = KyNang.Doc;
    public string? UrlHinh { get; set; }
    public string? UrlAmThanh { get; set; }
    public string? DapAnDung { get; set; }
    public string? GiaiThich { get; set; }
    public CapDo CapDo { get; set; } = CapDo.A1;
    public DoKho DoKho { get; set; } = DoKho.de;
    public Guid? DocHieuId { get; set; }
    public DocHieu? DocHieu { get; set; }

}