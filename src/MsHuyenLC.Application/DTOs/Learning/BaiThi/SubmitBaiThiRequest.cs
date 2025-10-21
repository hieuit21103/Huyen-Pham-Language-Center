namespace MsHuyenLC.Application.DTOs.Learning.BaiThi;

public class SubmitBaiThiRequest
{
    public Guid DeThiId { get; set; }
    public Guid HocVienId { get; set; }
    public List<CauTraLoiDto> CauTraLois { get; set; } = new List<CauTraLoiDto>();
}

public class CauTraLoiDto
{
    public Guid CauHoiId { get; set; }
    public string CauTraLoi { get; set; } = null!;
}
