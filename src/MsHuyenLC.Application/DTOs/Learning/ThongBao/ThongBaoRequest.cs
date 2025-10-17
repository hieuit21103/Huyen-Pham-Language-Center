using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.ThongBao;

public class ThongBaoRequest
{
    public Guid? LopHocId { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public DoiTuongNhan DoiTuongNhan { get; set; }
}
