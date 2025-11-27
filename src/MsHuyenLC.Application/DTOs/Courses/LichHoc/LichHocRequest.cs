namespace MsHuyenLC.Application.DTOs.Courses.LichHoc;

public class LichHocRequest
{
    public Guid LopHocId { get; set; }
    public Guid PhongHocId { get; set; }
    public List<ThoiGianBieuRequest> ThoiGianBieus { get; set; } = new List<ThoiGianBieuRequest>();
}
