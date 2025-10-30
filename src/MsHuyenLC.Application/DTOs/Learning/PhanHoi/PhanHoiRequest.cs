namespace MsHuyenLC.Application.DTOs.Learning.PhanHoi;

public class PhanHoiRequest
{
    public Guid HocVienId { get; set; }
    public string LoaiPhanHoi { get; set; } = null!;
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
}
