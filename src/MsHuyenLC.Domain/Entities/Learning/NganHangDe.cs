namespace MsHuyenLC.Domain.Entities.Learning;

public class NganHangDe
{
    public Guid Id { get; set; }
    public string NoiDung { get; set; } = null!;
    public LoaiCauHoi LoaiCauHoi { get; set; } = LoaiCauHoi.TracNghiem;
    public string? UrlAmThanh { get; set; }
    public string? DapAnDung { get; set; }
    public MucDo MucDo { get; set; } = MucDo.de;
}