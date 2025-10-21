namespace MsHuyenLC.Application.DTOs.Learning.BaiThi;

public class ChamBaiRequest
{
    public List<ChamCauHoiDto> CauHois { get; set; } = new List<ChamCauHoiDto>();
    public string? NhanXetChung { get; set; }
}

public class ChamCauHoiDto
{
    public Guid BaiThiChiTietId { get; set; }
    public float Diem { get; set; }
    public string? NhanXet { get; set; }
}
