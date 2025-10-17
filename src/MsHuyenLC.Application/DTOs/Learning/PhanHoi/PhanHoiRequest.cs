namespace MsHuyenLC.Application.DTOs.Learning.PhanHoi;

public class PhanHoiRequest
{
    public Guid HocVienId { get; set; }
    public Guid GiaoVienId { get; set; }
    public string NoiDung { get; set; } = null!;
}
