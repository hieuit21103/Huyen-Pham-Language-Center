namespace MsHuyenLC.Application.DTOs.Courses.PhongHoc;

public class PhongHocResponse
{
    public Guid Id { get; set; }
    public string TenPhong { get; set; } = null!;
    public int SoGhe { get; set; }
    public string? ViTri { get; set; }
}
