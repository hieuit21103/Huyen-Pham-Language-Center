namespace MsHuyenLC.Application.DTOs.Courses.PhongHoc;

public class PhongHocResponse
{
    public Guid Id { get; set; }
    public string TenPhong { get; set; } = null!;
    public int SucChua { get; set; }
    public string? ViTri { get; set; }
}
