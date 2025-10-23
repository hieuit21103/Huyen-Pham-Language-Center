using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;

public class NhomCauHoiChiTietRequest
{
    public Guid NhomId { get; set; }
    public Guid CauHoiId { get; set; }
    public int ThuTu { get; set; }
}

