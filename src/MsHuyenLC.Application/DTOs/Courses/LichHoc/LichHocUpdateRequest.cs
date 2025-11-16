namespace MsHuyenLC.Application.DTOs.Courses.LichHoc;

public class LichHocUpdateRequest
{
    public Guid PhongHocId { get; set; }
    public bool CoHieuLuc { get; set; }
    public List<ThoiGianBieuRequest> ThoiGianBieus { get; set; } = new List<ThoiGianBieuRequest>();
}
